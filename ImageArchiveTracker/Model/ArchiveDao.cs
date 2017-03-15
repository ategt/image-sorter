using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ImageArchiveTracker.Model
{
    public class ArchiveDao
    {
        SQLiteConnection m_dbConnection;

        public ArchiveDao(String databaseName)
        {
            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);
            }

            m_dbConnection = new SQLiteConnection($"Data Source={databaseName};Version=3; FailIfMissing=True; Foreign Keys=True;");
            m_dbConnection.Open();

            //string CREATE_ARCHIVE_TABLE = "CREATE TABLE IF NOT EXISTS media_archive_log (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, hash BLOB UNIQUE, filename STRING, flickr INT DEFAULT 0, google_photo INT DEFAULT 0, disc INT DEFAULT 0);";
            string CREATE_ARCHIVE_TABLE = "CREATE TABLE IF NOT EXISTS media_archive_log (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, hash BLOB UNIQUE, flickr INT DEFAULT 0, google_photo INT DEFAULT 0, disc INT DEFAULT 0);";
            SQLiteCommand createLogCommand = new SQLiteCommand(CREATE_ARCHIVE_TABLE, m_dbConnection);
            createLogCommand.ExecuteNonQuery();

            string CREATE_ARCHIVE_HASH_INDEX = "CREATE INDEX IF NOT EXISTS media_archive_log_hash_idx ON media_archive_log (hash);";
            SQLiteCommand createIndexCommand = new SQLiteCommand(CREATE_ARCHIVE_HASH_INDEX, m_dbConnection);
            createIndexCommand.ExecuteNonQuery();

        }

        public void Close()
        {
            m_dbConnection.Close();
        }

        public Archive Create(Archive archive)
        {
            //int result = -1;
            //Int64 lastID;
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection)) // , getLastRow = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "INSERT INTO media_archive_log (hash, flickr, google_photo, disc) VALUES (@Hash, @Flickr, @Google, @Disc);";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", archive.Hash);
                cmd.Parameters.AddWithValue("@Flickr", archive.Flickr);
                cmd.Parameters.AddWithValue("@Google", archive.GooglePhoto);
                cmd.Parameters.AddWithValue("@Disc", archive.Disc);

                int result = cmd.ExecuteNonQuery();
                //return result == 1;
                //getLastRow.CommandText = "SELECT last_insert_rowid();";
                //getLastRow.Prepare();

                //try
                //{
                //result = cmd.ExecuteNonQuery();
                //object returnValue = getLastRow.ExecuteScalar();
                //lastID = (Int64)returnValue;
                //sourceFile.Id = lastID;
                //}
                //catch (SQLiteException e)
                //{
                //}

                return (result == 1) ? archive : null;

            }
        }

        public Archive Get(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "SELECT hash, flickr, google_photo, disc FROM media_archive_log WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                SQLiteDataReader dataReader = cmd.ExecuteReader();

                Archive archive = null;

                while (dataReader.Read())
                {
                    archive.Hash = (byte[]) dataReader["hash"];
                    archive.GooglePhoto = (bool) dataReader["google_photo"];
                    archive.Flickr = (bool) dataReader["flickr"];
                    archive.Disc = (bool) dataReader["disc"];
                }

                return archive;
            }
        }

        public bool Add(byte[] hash) //, String filename = null)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                //if (filename == null)
                //{
                cmd.CommandText = "INSERT INTO media_archive_log (hash) VALUES (@Hash);";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);
                //}
                //else
                //{
                //    cmd.CommandText = "INSERT INTO media_archive_log (hash, filename) VALUES (@Hash, @Filename);";
                //    cmd.Prepare();
                //    cmd.Parameters.AddWithValue("@Hash", hash);
                //    cmd.Parameters.AddWithValue("@Filename", filename);
                //}

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }

        public bool AddedToFlickr(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "UPDATE media_archive_log SET flickr = 1 WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }

        public bool AddedToGooglePhoto(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "UPDATE media_archive_log SET google_photo = 1 WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }

        public bool AddedToDisc(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "UPDATE media_archive_log SET disc = 1 WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }

        public bool IsOnDisc(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM media_archive_log WHERE hash = @Hash AND disc = 1;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                object result = cmd.ExecuteScalar();
                return (int)result == 1;
            }
        }

        public bool IsOnFlickr(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM media_archive_log WHERE hash = @Hash AND flickr = 1;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                object result = cmd.ExecuteScalar();
                return (int)result == 1;
            }
        }

        public bool IsOnGooglePhoto(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM media_archive_log WHERE hash = @Hash AND google_photo = 1;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                object result = cmd.ExecuteScalar();
                return (int)result == 1;
            }
        }

        public bool RemovedFromFlickr(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "UPDATE media_archive_log SET flickr = 0 WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }

        public bool RemovedFromGooglePhoto(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "UPDATE media_archive_log SET google_photo = 0 WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }

        public bool RemovedFromDisc(byte[] hash)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(m_dbConnection))
            {
                cmd.CommandText = "UPDATE media_archive_log SET disc = 0 WHERE hash = @Hash;";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Hash", hash);

                int result = cmd.ExecuteNonQuery();
                return result == 1;
            }
        }
    }
}
