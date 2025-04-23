using _Kurs_10_Dapper.Context;
using _Kurs_10_Dapper.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;

namespace _Kurs_10_Dapper.Respositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly DapperContext _context;

        public SalesRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        public async Task<List<BrandPreferenceViewModel>> BrandPreferenceInfoAsync()
        {
            var query = @"SELECT 
                    BRAND, 
                    COUNT(*) AS PreferredCount 
                  FROM SALES 
                  GROUP BY BRAND 
                  ORDER BY PreferredCount DESC OFFSET 3 ROWS FETCH NEXT 10 ROWS ONLY";

            using var connection = _context.CreateConnection();
            var result = (await connection.QueryAsync<BrandPreferenceViewModel>(query, commandTimeout: 300)).ToList();
            return result;

        }

        public async Task<List<GetAllinfoViewModel>> GetAllInfoAsync()
        {
            var query = "SELECT ID, DATE_,NAMESURNAME, ITEMNAME, AMOUNT, PRICE, TOTALPRICE, CITY, TOWN FROM SALES";
            var connection = _context.CreateConnection();
            var values = (await connection.QueryAsync<GetAllinfoViewModel>(query, commandTimeout: 300)).ToList();
            return values;
        }

        public async Task<int> GetAllSalesCount()
        {
            string query = "SELECT COUNT(*) FROM SALES";
            var connection = _context.CreateConnection();
            var values = await connection.QueryFirstOrDefaultAsync<int>(query, commandTimeout: 300);
            return values;
        }

        public async Task<List<GenderAgeGroupViewModel>> GetGenderAgeGroupStatsAsync()
        {
            var query = @"SELECT 
            CASE 
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1950 AND 1959 THEN '1950-1959'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1960 AND 1969 THEN '1960-1969'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1970 AND 1979 THEN '1970-1979'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1980 AND 1989 THEN '1980-1989'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1990 AND 1999 THEN '1990-1999'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 2000 AND 2010 THEN '2000-2009'
                ELSE 'Diğer'
            END AS AgeGroup,
            SUM(CASE WHEN USERGENDER = 'E' THEN 1 ELSE 0 END) AS MaleCount,
            SUM(CASE WHEN USERGENDER = 'K' THEN 1 ELSE 0 END) AS FemaleCount
        FROM SALES
        GROUP BY 
            CASE 
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1950 AND 1959 THEN '1950-1959'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1960 AND 1969 THEN '1960-1969'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1970 AND 1979 THEN '1970-1979'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1980 AND 1989 THEN '1980-1989'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 1990 AND 1999 THEN '1990-1999'
                WHEN YEAR(USERBIRTHDATE) BETWEEN 2000 AND 2010 THEN '2000-2009'
                ELSE 'Diğer'
            END
        ORDER BY AgeGroup";
            var connection = _context.CreateConnection();
            var result = (await connection.QueryAsync<GenderAgeGroupViewModel>(query, commandTimeout: 300)).ToList();
            return result;

        }

        // Aylık satışları alır
        public async Task<List<MonthlySalesViewModel>> GetMonthlySalesAsync()
        {
            string query = @"SELECT 
                FORMAT(DATE_, 'yyyy-MM') AS SalesMonth,
                CAST(SUM(TOTALPRICE)/1000000 AS INT) AS TotalSales
                FROM SALES
                GROUP BY FORMAT(DATE_, 'yyyy-MM')
                ORDER BY SalesMonth;";

            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<MonthlySalesViewModel>(query, commandTimeout: 300);
            return result.ToList();
        }

        public async Task<List<BrandSalesViewModel>> GetSalesDataByBrand()
        {
            string query = @"SELECT BRAND,
                CASE 
                    WHEN SUM(AMOUNT) >= 1000000000 THEN CAST(SUM(AMOUNT) AS INT)
                    WHEN SUM(AMOUNT) >= 1000000 THEN CAST(SUM(AMOUNT) AS INT)
                    WHEN SUM(AMOUNT) >= 1000 THEN CAST(SUM(AMOUNT) AS INT)
                    ELSE CAST(SUM(AMOUNT) AS INT)
                END AS TotalSold,
                CASE 
                    WHEN SUM(AMOUNT) >= 1000000000 THEN 'B'
                    WHEN SUM(AMOUNT) >= 1000000 THEN 'M'
                    WHEN SUM(AMOUNT) >= 1000 THEN 'K'
                    ELSE ''
                END AS Unit
                FROM SALES
                GROUP BY BRAND
                ORDER BY SUM(AMOUNT) DESC
                OFFSET 3 ROWS FETCH NEXT 10 ROWS ONLY";

            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<BrandSalesViewModel>(query, commandTimeout: 300);
            return result.ToList();

        }

        public async Task<List<CityOrderViewModel>> GetTop10CitiesByOrderAsync()
        {
            var query = @"SELECT TOP(10) CITY, COUNT(*) AS OrderCount
                  FROM SALES
                  GROUP BY CITY
                  ORDER BY OrderCount DESC;";
            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<CityOrderViewModel>(query, commandTimeout: 300);
            return result.ToList();
        }

        public async Task<List<AgeRangeViewModel>> GetUserCountByAgeRangeAsync()
        {
            string sql = @"SELECT 
                CASE 
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1950 AND 1959 THEN '1950-1959'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1960 AND 1969 THEN '1960-1969'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1970 AND 1979 THEN '1970-1979'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1980 AND 1989 THEN '1980-1989'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1990 AND 1999 THEN '1990-1999'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 2000 AND 2009 THEN '2000-2009'
                    WHEN YEAR(USERBIRTHDATE) >= 2010 THEN '2010+'
                    ELSE 'Bilinmiyor'
                END AS AgeRange,
                COUNT(*) AS UserCount
                FROM SALES
                WHERE USERBIRTHDATE IS NOT NULL
                GROUP BY 
                CASE 
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1950 AND 1959 THEN '1950-1959'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1960 AND 1969 THEN '1960-1969'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1970 AND 1979 THEN '1970-1979'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1980 AND 1989 THEN '1980-1989'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 1990 AND 1999 THEN '1990-1999'
                    WHEN YEAR(USERBIRTHDATE) BETWEEN 2000 AND 2009 THEN '2000-2009'
                    WHEN YEAR(USERBIRTHDATE) >= 2010 THEN '2010+'
                    ELSE 'Bilinmiyor'
                END
            ORDER BY AgeRange";
            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<AgeRangeViewModel>(sql, commandTimeout: 300);
            return result.ToList();
        }

        public async Task<List<GetAllinfoViewModel>> SearchProductsAsync(string keyword)
        {
            string query = @"SELECT ID, DATE_,NAMESURNAME, ITEMNAME, AMOUNT, PRICE, TOTALPRICE, CITY,                TOWN FROM SALES WHERE NAMESURNAME LIKE @Keyword";

            var parameters = new { Keyword = "%" + keyword + "%" };
            
            var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<GetAllinfoViewModel>(query, parameters, commandTimeout: 300);
            return result.ToList();
        }
    }
}