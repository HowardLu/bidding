using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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

    public enum AuctionColumnHeader
    {
        拍品編號 = 0,
        拍品名稱,
        尺寸,
        得標牌號,
        庫存狀態,
        賣家,
        歸還狀態,
        落槌價,
        買家適用服務費,
        成交價,
        保證金繳納,
        保證金繳納金額,
        保證金退還,
        保證金退還金額,
        付款方式,
        賣家服務及保險費,
        保留價,
        應負賣家金額,
        Count
    }

    public class AuctionEntityTW
    {
        public ObjectId Id { get; set; }
        public string 拍品編號 { get; set; }
        public string 拍品名稱 { get; set; }
        public string 尺寸 { get; set; }
        public string 得標牌號 { get; set; }
        public string 庫存狀態 { get; set; }
        public string 賣家 { get; set; }
        public int 歸還狀態 { get; set; }
        public int 落槌價 { get; set; }
        public int 買家適用服務費 { get; set; }
        public int 成交價 { get; set; }
        public int 保證金繳納 { get; set; }
        public int 保證金繳納金額 { get; set; }
        public int 保證金退還 { get; set; }
        public int 保證金退還金額 { get; set; }
        public int 付款方式 { get; set; }
        public int 賣家服務及保險費 { get; set; }
        public int 保留價 { get; set; }
        public int 應負賣家金額 { get; set; }
    }

    public class Internet<TEntity>
    {
        #region Enum, Internal Class
        #endregion

        #region Events
        #endregion

        #region Member Variables
        private MongoClient m_client;
        private String m_connectionStr = "mongodb://localhost:27017";
        private String m_dbName = "test";
        private String m_collectionName = "Auctions";
        private MongoCollection<TEntity> m_collection;
        private List<TEntity> m_collectionList = null;
        #endregion

        #region Properties
        public String ConnectionString { get { return m_connectionStr; } set { m_connectionStr = value; } }
        public String DbName { get { return m_dbName; } set { m_dbName = value; } }
        public String CollectionName { get { return m_collectionName; } set { m_collectionName = value; } }
        public MongoCollection<TEntity> Collection { get { return m_collection; } }
        #endregion

        #region Constructors
        public Internet(string connectionString, string dbName, string collectionName)
        {
            m_connectionStr = connectionString;
            m_dbName = dbName;
            m_collectionName = collectionName;
        }
        #endregion

        #region Windows Form Events
        #endregion

        #region Public Methods
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

        public void UpdateStockState(string auctionId, string stockState)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            IMongoUpdate update = Update<AuctionEntity>.Set(e => e.StockState, stockState);
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

        public List<TEntity> GetCollectionList()
        {
            if(m_collectionList == null)
                m_collectionList = m_collection.FindAll().ToList<TEntity>();

            return m_collectionList;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
