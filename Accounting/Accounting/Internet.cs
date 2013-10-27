using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Accounting
{
    public class Entity
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }

    public class Internet
    {
        private String m_configFileName = "config.txt";
        private String m_connectionStr = "mongodb://1.34.233.143:27017";
        private MongoClient m_client;
        private String m_dbName = "test";
        private String m_collectionName = "entities";

        public Internet()
        {
            
        }

        public void Connect()
        {
            m_client = new MongoClient(m_connectionStr);
        }

        public void Read()
        {
            MongoServer server = m_client.GetServer();
            MongoDatabase database = server.GetDatabase(m_dbName);
            MongoCollection<Entity> collection = database.GetCollection<Entity>(m_collectionName);
            var entity = new Entity { Name = "Tom" };
            collection.Insert(entity);
            collection.Save(entity);
        }

        private bool LoadConnectionStr()
        {
            String fn = Path.Combine(System.Windows.Forms.Application.StartupPath, m_configFileName);
            if (File.Exists(fn))
            {

            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
