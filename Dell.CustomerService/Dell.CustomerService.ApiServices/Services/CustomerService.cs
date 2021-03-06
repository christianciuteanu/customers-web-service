﻿using System;
using System.Threading.Tasks;
using Dell.CustomerService.Domain;
using Dell.CustomerService.Domain.Data.Entities;
using Dell.CustomerService.Domain.Repositories;
using Dell.CustomerService.Web.ApiContracts.Customers;

namespace Dell.CustomerService.Web.ApiServices.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly IUnitOfWork _unitOfWork;

		public CustomerService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<CustomerResponseData> AddUpdateCustomerAsync(CustomerRequestData requestData)
		{
			var existingCustomer = await _unitOfWork.CustomerRepository.GetCustomerByEmailAsync(requestData.Email);

			if (existingCustomer != null)
			{
				return UpdateCustomerAsync(existingCustomer, requestData);
			}

			return await AddCustomerAsync(requestData);
		}

		private async Task<CustomerResponseData> AddCustomerAsync(CustomerRequestData requestData)
		{
			await _unitOfWork.CustomerRepository.Create(new Customer
			{
				Name = requestData.Name,
				Email = requestData.Email
			});

			var createdCustomer = await _unitOfWork.CustomerRepository.GetCustomerByEmailAsync(requestData.Email);
			return new CustomerResponseData
			{
				Email = createdCustomer.Email,
				Name = createdCustomer.Name,
				Id = createdCustomer.Id
			};
		}

		private CustomerResponseData UpdateCustomerAsync(Customer dbCustomer, CustomerRequestData requestData)
		{
			dbCustomer.Name = requestData.Name;
			_unitOfWork.CustomerRepository.Update(dbCustomer);
			return new CustomerResponseData
			{
				Id = dbCustomer.Id,
				Name = dbCustomer.Name,
				Email = dbCustomer.Email
			};
		}
	}
}