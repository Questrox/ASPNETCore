﻿using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Сервис для управления строками дополнительных услуг
    /// </summary>
    public class ServiceStringService
    {
        private readonly IServiceStringRepository _serviceStringRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAdditionalServiceRepository _additionalServiceRepository;
        private readonly ILogger<ServiceStringService> _logger;

        public ServiceStringService(IServiceStringRepository serviceStringRepository, IAdditionalServiceRepository additionalServiceRepository,
            IReservationRepository reservationRepository, ILogger<ServiceStringService> logger)
        {
            _serviceStringRepository = serviceStringRepository;
            _additionalServiceRepository = additionalServiceRepository;
            _reservationRepository = reservationRepository;
            _logger = logger;
        }
        /// <summary>
        /// Оказывает дополнительную услугу (обновляет столбец оказанных услуг).
        /// Также проверяет корректность данных и обновляет цены связанного бронирования
        /// </summary>
        /// <param name="serviceStringID">Идентификатор оказываемой строки услуг</param>
        /// <param name="amount">Количество оказываемых услуг</param>
        /// <returns>Обновленную строку услуги</returns>
        /// <exception cref="ArgumentException">Исключение на случай неправильности введенных данных</exception>
        public async Task<ServiceStringDTO> DeliverServiceAsync(int serviceStringID, int amount)
        {
            //Проверки
            if (amount <= 0)
            {
                _logger.LogError($"Ошибка оказания услуги: введено количество услуг <= 0 ({amount})"); 
                throw new ArgumentException("Попытка указать количество услуг <= 0: " + amount);
            }
            var servStr = await _serviceStringRepository.GetServiceStringByIdAsync(serviceStringID);
            if (servStr == null)
            {
                _logger.LogError($"Ошибка оказания услуги: строка услуг с идентификатором {serviceStringID} не найдена");
                throw new ArgumentException("Не существует строки услуг с идентификатором " + serviceStringID);
            }
            var res = await _reservationRepository.GetReservationByIdAsync(servStr.ReservationID);
            var service = await _additionalServiceRepository.GetAdditionalServiceByIdAsync(servStr.AdditionalServiceID);
            if (servStr.Count - servStr.DeliveredCount < amount)
            {
                _logger.LogError($"Ошибка оказания услуги: нельзя оказать больше услуг, чем было забронировано " +
                    $"(забронировано {servStr.Count}, уже оказано {servStr.DeliveredCount}, попытка оказать {amount})");
                throw new ArgumentException("Попытка оказать больше услуг, чем было забронировано");
            }
            if (servStr.ServiceStatusID == 2)
            {
                _logger.LogError($"Ошибка оказания услуги: услуга {serviceStringID} уже оплачена");
                throw new ArgumentException("Попытка оказать уже оказанную (оплаченную) услугу");
            }
            if (servStr.ServiceStatusID == 3)
            {
                _logger.LogError($"Ошибка оказания услуги: услуга {serviceStringID} отменена");
                throw new ArgumentException("Попытка оказать отмененную услугу");
            }
            if (res.ReservationStatusID == 1)
            {
                _logger.LogError($"Ошибка оказания услуги: гость в бронировании с id {res.ID} еще не заехал");
                throw new ArgumentException("Попытка оказать услугу гостю, который еще не заехал");
            }
            if (res.ReservationStatusID == 3)
            {
                _logger.LogError($"Ошибка оказания услуги: гость в бронировании с id {res.ID} уже выехал");
                throw new ArgumentException("Попытка оказать услугу гостю, который уже выехал");
            }
            if (res.ReservationStatusID == 4)
            {
                _logger.LogError($"Ошибка оказания услуги: бронирование с id {res.ID} отменено");
                throw new ArgumentException("Попытка оказать услугу в отмененном бронировании");
            }

            //Обновляем столбец количества оказанных услуг
            servStr.DeliveredCount += amount;
            await _serviceStringRepository.UpdateServiceStringAsync(servStr);

            //Обновляем цену бронирования (и за услуги, и полную)
            res.ServicesPrice += service.Price * amount;
            res.FullPrice += service.Price * amount;
            await _reservationRepository.UpdateReservationAsync(res);

            var result = await _serviceStringRepository.GetServiceStringByIdAsync(serviceStringID);
            return new ServiceStringDTO(result);
        }
        /// <summary>
        /// Получает список всех строк услуг
        /// </summary>
        /// <returns>Список всех строк услуг</returns>
        public async Task<IEnumerable<ServiceStringDTO>> GetServiceStringsAsync()
        {
            var strings = await _serviceStringRepository.GetServiceStringsAsync();
            return strings.Select(x => new ServiceStringDTO(x));
        }
        /// <summary>
        /// Получает строку услуги по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор строки услуги</param>
        /// <returns>Строка услуги или null, если не найдена</returns>
        public async Task<ServiceStringDTO> GetServiceStringByIdAsync(int id)
        {
            var s = await _serviceStringRepository.GetServiceStringByIdAsync(id);
            if (s == null)
            {
                _logger.LogError($"Не удалось найти строку услуги с идентификатором {id}");
                return null;
            }
            return new ServiceStringDTO(s);
        }
        /// <summary>
        /// Добавляет новую строку услуги
        /// </summary>
        /// <param name="createServiceStringDTO">Добавляемая строка</param>
        /// <returns>Созданная строка услуги</returns>
        /// <exception cref="ArgumentException">Если указаны неверные параметры</exception>
        public async Task<ServiceStringDTO> AddServiceStringAsync(CreateServiceStringDTO createServiceStringDTO)
        {
            var service = await _additionalServiceRepository.GetAdditionalServiceByIdAsync(createServiceStringDTO.AdditionalServiceID);
            if (service == null)
            {
                _logger.LogError($"Ошибка добавления строки услуги: не найдена доп.услуга " +
                    $"с идентификатором {createServiceStringDTO.AdditionalServiceID}");
                throw new ArgumentException("Не найдена доп.услуга с идентификатором " + createServiceStringDTO.AdditionalServiceID);
            }
            if (createServiceStringDTO.Count < 1)
            {
                _logger.LogError($"Ошибка добавления строки услуги: неверно указано количество услуг ({createServiceStringDTO.Count})");
                throw new ArgumentException("Неверно указано количество услуг (Count = " + createServiceStringDTO.Count + ")");
            }
            var s = new ServiceString
            {
                Count = createServiceStringDTO.Count,
                DeliveredCount = 0,
                AdditionalServiceID = createServiceStringDTO.AdditionalServiceID,
                ReservationID = createServiceStringDTO.ReservationID,
                Price = service.Price * createServiceStringDTO.Count,
                ServiceStatusID = createServiceStringDTO.ServiceStatusID
            };

            await _serviceStringRepository.AddServiceStringAsync(s);

            return new ServiceStringDTO(s);
        }
        /// <summary>
        /// Обновляет строку услуги
        /// </summary>
        /// <param name="s">Обновляемая строка</param>
        /// <returns>Обновленная строка или null, если не найдена</returns>
        public async Task<ServiceStringDTO?> UpdateServiceStringAsync(ServiceStringDTO s)
        {
            var existingString = await _serviceStringRepository.GetServiceStringByIdAsync(s.ID);
            if (existingString == null)
            {
                _logger.LogError($"Строка услуг с id {s.ID} не найден, обновление не выполнено");
                return null;
            } 

            existingString.Count = s.Count;
            existingString.DeliveredCount = s.DeliveredCount;
            existingString.AdditionalServiceID = s.AdditionalServiceID;
            existingString.ReservationID = s.ReservationID;
            existingString.Price = s.Price;
            existingString.ServiceStatusID = s.ServiceStatusID;

            await _serviceStringRepository.UpdateServiceStringAsync(existingString);

            return new ServiceStringDTO(existingString); // Возвращаем обновленный объект
        }
        /// <summary>
        /// Удаляет строку услуги по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор строки услуги</param>
        public async Task DeleteServiceStringAsync(int id)
        {
            await _serviceStringRepository.DeleteServiceStringAsync(id);
        }
    }
}