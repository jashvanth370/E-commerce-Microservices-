using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //check if product already exists
                var getProduct = await GetByAsync(_=> _.Name.Equals(entity.Name));
                if(getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} Product already exists.");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if(currentEntity.Id > 0 && currentEntity is not null)
                    return new Response(true, $"{currentEntity} Product created and added successfully.");
                else
                    return new Response(false, $"Product creation failed.{entity.Name}");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display scary-free message to the client
                return new Response(false, "An error occurred while creating the product.");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                    return new Response(false, $"{entity.Name} Product not found.");
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} Product deleted successfully.");

            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display scary-free message to the client
                return new Response(false, "An error occurred while deleting the product.");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display scary-free message to the client
                throw new Exception("An error occurred while retriving the product.");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display scary-free message to the client
                throw new Exception("An error occurred while retriving the products.");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display scary-free message to the client
                throw new Exception("An error occurred while retriving the product.");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} Product not found.");
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} Product updated successfully.");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display scary-free message to the client
                return new Response(false, "An error occurred while updating the product.");
            }
        }
    }
}
