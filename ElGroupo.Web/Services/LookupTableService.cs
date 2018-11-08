using ElGroupo.Web.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using ElGroupo.Web.Models.Shared;

namespace ElGroupo.Web.Services
{
    public class LookupTableService
    {
        private readonly ElGroupo.Domain.Data.ElGroupoDbContext _ctx;
        public LookupTableService(ElGroupo.Domain.Data.ElGroupoDbContext ctx)
        {
            //ctx.Database. 
            _ctx = ctx;
            //this.Tables = new Dictionary<string, List<IdValueModel>>();
            //foreach (var item in ctx.RecordElementLookupTables)
            //{
            //    this.Tables.Add(item.TableName, new List<IdValueModel>());
            //}
            //var sqlCmd = new SqlCommand { Connection = ctx.Database.GetDbConnection() as SqlConnection };
            //foreach (var item in ctx.RecordElementLookupTables)
            //{
            //    sqlCmd.CommandText = "select * from " + item.TableName + " order by Value asc";
            //    this.Tables.Add(item.TableName, new List<IdValueModel>());
            //    try
            //    {
            //        var reader = sqlCmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            this.Tables[item.TableName].Add(new IdValueModel
            //            {
            //                Id = Convert.ToInt32(reader["Id"]),
            //                Value = reader["Value"].ToString()
            //            });
            //        }
            //        reader.Close();
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}
        }

        public List<IdValueModel> GetValuesByLookupTableId(long id)
        {
            var item = _ctx.RecordElementLookupTables.First(x => x.Id == id);
            return this.LoadTable(item.TableName);


            //return new List<IdValueModel>();
        }
        public List<IdValueModel> GetValuesByLookupTableName(string name)
        {
            if (name == null) return null;
            return this.LoadTable(name);

        }

        public SaveDataResponse CreateTable(string tableName)
        {
            var conn = _ctx.Database.GetDbConnection() as SqlConnection;
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            var sqlCmd = new SqlCommand
            {
                Connection = conn,
                CommandType = System.Data.CommandType.Text,
                CommandText = "select count(*) from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName.ToUpper().ToString() + "'"
            };
            var count = Convert.ToInt32(sqlCmd.ExecuteScalar());
            if (count > 0) return SaveDataResponse.FromErrorMessage("Table Already Exists");

            sqlCmd.CommandText = "create table " + tableName + " (id int identity(1,1) not null primary key, [value] varchar(100))";
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                return SaveDataResponse.FromException(sqlEx);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
            finally
            {
                conn.Close();
            }
            //this.Tables.Add(tableName, new List<IdValueModel>());
            return SaveDataResponse.Ok();
        }
        public List<IdValueModel> LoadTable(string tableName)
        {
            var list = new List<IdValueModel>();
            var sqlCmd = new SqlCommand { Connection = _ctx.Database.GetDbConnection() as SqlConnection };
            if (sqlCmd.Connection.State != System.Data.ConnectionState.Open) sqlCmd.Connection.Open();
            sqlCmd.CommandText = "select * from " + tableName + " order by Value asc";
            try
            {
                var reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new IdValueModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Value = reader["Value"].ToString()
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {

            }
            return list;

        }
        public SaveDataResponse DropTable(string tableName)
        {
            var conn = _ctx.Database.GetDbConnection() as SqlConnection;
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();


            var sqlCmd = new SqlCommand
            {
                Connection = conn,
                CommandType = System.Data.CommandType.Text,
                CommandText = "select count(*) from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName.ToUpper().ToString() + "'"
            };
            var count = Convert.ToInt32(sqlCmd.ExecuteScalar());
            if (count > 0)
            {
                sqlCmd.CommandText = "drop table " + tableName;
                try
                {
                    sqlCmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (SqlException sqlEx)
                {
                    return SaveDataResponse.FromException(sqlEx);
                }
                catch (Exception ex)
                {
                    return SaveDataResponse.FromException(ex);
                }
            }


            //if (this.Tables.ContainsKey(tableName)) this.Tables.Remove(tableName);
            return SaveDataResponse.Ok();
        }

        private string FindLookupValue(string tableName, int id)
        {
            var conn = _ctx.Database.GetDbConnection() as SqlConnection;
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            using (var sqlCmd = new SqlCommand
            {
                Connection = conn,
                CommandType = System.Data.CommandType.Text,
                CommandText = "select value from " + tableName + " where id = " + id.ToString()
            })
            {
                using (var reader = sqlCmd.ExecuteReader())
                {
                    var foundRecord = reader.Read();
                    if (foundRecord)
                    {
                        //this.Tables[tableName].Add(new IdValueModel { Id = id, Value = reader[0].ToString() });
                        return reader[0].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }




        }
        public string GetValue(string tableName, object id)
        {
            if (id == null || tableName == null) return null;
            return this.FindLookupValue(tableName, Convert.ToInt32(id));
        }

        public List<IdValueModel> LookupSearch(string tableName, string searchText)
        {
            var list = new List<IdValueModel>();
            var conn = _ctx.Database.GetDbConnection() as SqlConnection;
            if (conn.State != System.Data.ConnectionState.Open) conn.Open();
            using (var sqlCmd = new SqlCommand
            {
                Connection = conn,
                CommandType = System.Data.CommandType.Text,
                CommandText = "select * from " + tableName + " where value like '%" + searchText + "%' order by value asc"
            })
            {
                using (var reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new IdValueModel { Id = Convert.ToInt32(reader[0]), Value = reader[1].ToString() });
                    }
                }
            }


            return list;
            //if (!this.Tables.ContainsKey(tableName.ToUpper())) return list;
            //return this.Tables[tableName.ToUpper()].Where(x => x.Value.ToUpper().Contains(searchText.ToUpper())).ToList();
        }
        public SaveDataResponse AddValue(string tableName, string value)
        {
            try
            {
                var conn = _ctx.Database.GetDbConnection() as SqlConnection;
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                var sqlCmd = new SqlCommand
                {
                    Connection = conn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "insert into " + tableName + " ([Value]) values('" + value + "'); select SCOPE_IDENTITY()"
                };
                var newId = Convert.ToInt32(sqlCmd.ExecuteScalar());
                //if (!this.Tables.ContainsKey(tableName)) this.Tables.Add(tableName, new List<IdValueModel>());
                //this.Tables[tableName].Add(new IdValueModel { Id = newId, Value = value });
                conn.Close();
                return SaveDataResponse.IncludeData(newId);


            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public SaveDataResponse RemoveValue(string tableName, long id)
        {
            try
            {
                var conn = _ctx.Database.GetDbConnection() as SqlConnection;
                if (conn.State != System.Data.ConnectionState.Open) conn.Open();
                var sqlCmd = new SqlCommand
                {
                    Connection = conn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "delete from " + tableName + " where Id = " + id
                };
                sqlCmd.ExecuteNonQuery();
                //if (this.Tables.ContainsKey(tableName))
                //{
                //    var item = this.Tables[tableName].FirstOrDefault(x => x.Id == id);
                //    if (item != null) this.Tables[tableName].Remove(item);
                //}
                conn.Close();
                return SaveDataResponse.Ok();


            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        //public Dictionary<string, List<IdValueModel>> Tables { get; set; }
    }
}
