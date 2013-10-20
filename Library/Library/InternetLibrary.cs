using System;
using System.IO;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace InternetLibrary
{
    public class AuctionEntity
    {
        public ObjectId Id { get; set; }
        public string AuctionId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string BidderNumber { get; set; }
        public string StockState { get; set; }
        public string Seller { get; set; }
        public int ReturnState { get; set; }
        public int HammerPrice { get; set; }
        public int BuyerServiceCharge { get; set; }
        public int FinalPrice { get; set; }
        public int PayGuaranteeState { get; set; }
        public int PayGuaranteeNumber { get; set; }
        public int ReturnGuaranteeState { get; set; }
        public int ReturnGuaranteeNumber { get; set; }
        public int PayWayState { get; set; }
        public int SellerServiceCharge { get; set; }
        public int ReservePrice { get; set; }
        public int SellerAccountPayable { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class Internet<TEntity>
    {
        private MongoClient m_client;
        private String m_connectionStr = "mongodb://localhost:27017";
        private String m_dbName = "test";
        private String m_collectionName = "Auctions";
        private MongoCollection<TEntity> m_collection;

        public String ConnectionString { get { return m_connectionStr; } set { m_connectionStr = value; } }
        public String DbName { get { return m_dbName; } set { m_dbName = value; } }
        public String CollectionName { get { return m_collectionName; } set { m_collectionName = value; } }
        public MongoCollection<TEntity> Collection { get { return m_collection; } }

        public Internet(string connectionString, string dbName, string collectionName)
        {
            m_connectionStr = connectionString;
            m_dbName = dbName;
            m_collectionName = collectionName;
        }

        public void Connect()
        {
            m_client = new MongoClient(m_connectionStr);
            MongoServer server = m_client.GetServer();
            /*if (server.State == MongoServerState.Disconnected)
                return;*/
            MongoDatabase database = server.GetDatabase(m_dbName);
            m_collection = database.GetCollection<TEntity>(m_collectionName);
        }

        public void UpdateName(string auctionId, string name)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            IMongoUpdate update = Update<AuctionEntity>.Set(e => e.Name, name);
            m_collection.Update(query, update);
        }

        public void Insert(TEntity entity)
        {
            m_collection.Insert(entity);
            WriteConcernResult wcr = m_collection.Save(entity);
            if (!wcr.Ok)
            {
                Console.WriteLine("[Mongo] Insert error!");
            }
        }
    }
}
