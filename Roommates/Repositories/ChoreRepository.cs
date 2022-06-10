using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roommates.Models;


namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Chore data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class ChoreRepository : BaseRepository
    {
        // <summary>
        // When a new chore repository is instantiated, it will pass along the connection string to the base Repo
        // </summary>
        public ChoreRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        public List<Chore> GetAll()
        {
            // connect
            using (SqlConnection conn = Connection)
            {
                // open connection
                conn.Open();
                // create a query
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    // read data from database a line at a time
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");
                            int idValue = reader.GetInt32(idColumnPosition);

                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            Chore chore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue
                            };

                            chores.Add(chore);
                        }

                        return chores;
                    }
                }
            }
        }

        /// <summary>
        ///  Returns a single Chore with the given id.
        /// </summary>
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;
                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                        return chore;
                    }
                }
            }
        }

        /// <summary>
        ///  Add a new room to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Insert INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }

            }
        }
        /// <summary>
        ///  Returns all unassigned chores
        /// </summary>
        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Name, c.Id
                                        FROM Chore c
                                        LEFT JOIN RoommateChore rc ON c.Id = rc.ChoreId
                                        WHERE rc.ChoreId IS NULL";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while(reader.Read())
                        {
                            chores.Add(new Chore
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                        return chores;
                    }
                }
            }

        }
        /// <summary>
        ///  Returns all assigned chores
        /// </summary>
        public List<Chore> GetAssignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Name, rc.Id
                                        FROM Chore c
                                        LEFT JOIN RoommateChore rc ON c.Id = rc.ChoreId
                                        WHERE rc.ChoreId IS NOT NULL";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while(reader.Read())
                        {
                            chores.Add(new Chore
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                        return chores;
                    }
                }
            }
        }
        // <summary>
        // returns a list of chores and who they are assigned to given a choreId
        // <summary>
        public Roommate GetAssignmentsByRoommateChoreId(int rcId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.FirstName, rm.Id From Roommate rm 
                                        LEFT JOIN RoommateChore rc ON rm.Id = rc.RoommateId 
                                        WHERE rc.Id = @cId";
                    cmd.Parameters.AddWithValue("@cId", rcId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Roommate roomie = null;
                        if(reader.Read())
                        {
                            roomie = new Roommate
                            {
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                Id = reader.GetInt32(reader.GetOrdinal("Id"))
                            };
                        }
                        return roomie;
                    }
                }
            }
        }

        // <summary>
        // assigns a chore to a roommate
        // <summary>
        public void AssignChore(int choreId, int roommateId)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId, ChoreId)
                                        VALUES (@rId, @cId)";
                    cmd.Parameters.AddWithValue("@cId", choreId);
                    cmd.Parameters.AddWithValue("@rId", roommateId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // <summary>
        // reassign a chore based on RoommateChore.Id and roommate
        // <summary>
        public void ReassignChore(int rcId, int roommateId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE RoommateChore
                                        SET RoommateId = @rId
                                        WHERE Id = @rcId";
                    cmd.Parameters.AddWithValue("@rcId", rcId);
                    cmd.Parameters.AddWithValue("@rId", roommateId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // <summary>
        // assigns a chore to a roommate
        // <summary>
        public Dictionary<string, int> GetChoreCounts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rm.FirstName, COUNT(rc.Id) AS ChoreCount
                                        FROM Roommate rm
                                        LEFT JOIN RoommateChore rc ON rm.Id = rc.RoommateId
                                        GROUP BY rm.FirstName";

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dictionary<string, int> counts = new Dictionary<string, int>();
                        while(reader.Read())
                        {
                            // roommate.chorecount property option:
                            // Roommate roomie = new Roommate
                            // {
                            //   FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            //   ChoreCount = reader.GetInt32(reader.GetOrdinal("ChoreCount"))
                            // }
                            // roommates.Add(roomie)
                            counts[reader.GetString(reader.GetOrdinal("FirstName"))] = reader.GetInt32(reader.GetOrdinal("ChoreCount"));
                        }
                        return counts;
                    }
                }
            }
        }

        /// <summary>
        ///  Updates the chore
        /// </summary>
        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                        SET Name = @name
                                        WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.Parameters.AddWithValue("@Id", chore.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Deletes the chore based on Id
        /// </summary>
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
