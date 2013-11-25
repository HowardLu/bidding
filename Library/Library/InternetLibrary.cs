using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public string Artist { get; set; }
        public string Artwork { get; set; }
        public int InitialPrice { get; set; }
        public int NowPrice { get; set; }
    }

    public class BidderEntity
    {
        public int _id { get; set; }
        public string BankContact { get; set; }
        public string Fax { get; set; }
        public int BidderID_int { get; set; }
        public string Tel { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string BankAcc { get; set; }
        public string CareerTitle { get; set; }
        public string IDNumber { get; set; }
        public string BankContactTel { get; set; }
        public string Address { get; set; }
        public string BidderID { get; set; }
        public string CreditCardID { get; set; }
        public string CreditCardType { get; set; }
        public string EMail { get; set; }
        public string Bank { get; set; }
    }

    public class SellerEntity
    {
        public string _id { get; set; }
        public string IfDealedInsuranceFee { get; set; }
        public string FrameFee { get; set; }
        public string Fax { get; set; }
        public string FireFee { get; set; }
        public string Tel { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string IdentifyFee { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string IfDealedPictureFee { get; set; }
        public string BankAcc { get; set; }
        public string IfNDealedPictureFee { get; set; }
        public string IfNDealedInsuranceFee { get; set; }
        public string CardID { get; set; }
        public string IfDealedServiceFee { get; set; }
        public string PostID { get; set; }
        public string IfNDealedServiceFee { get; set; }
        public string ContractID { get; set; }
    }

    public class BiddingResultEntity
    {
        public ObjectId Id { get; set; }
        public string BidderId { get; set; }
        public string AuctionId { get; set; }
        public int HammerPrice { get; set; }

        public BiddingResultEntity(string bidderId, string auctionId, int hammerPrice)
        {
            this.BidderId = bidderId;
            this.AuctionId = auctionId;
            this.HammerPrice = hammerPrice;
        }
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
        public int 應付賣家金額 { get; set; }
    }

    public class Internet<TEntity>
    {
        #region Enum, Internal Class
        #endregion

        #region Events
        #endregion

        #region Member Variables
        private MongoClient m_client;
        private String m_ip;
        private String m_port = "27017";
        private String m_connectionStr = "mongodb://localhost:27017";
        private String m_dbName = "test";
        private String m_collectionName = "Auctions";
        private MongoCollection<TEntity> m_collection;
        private List<TEntity> m_collectionList = null;
        #endregion

        #region Properties
        public String IP { get { return m_ip; } set { CombineConnString(value); } }
        public String ConnectionString { get { return m_connectionStr; } set { m_connectionStr = value; } }
        public bool IsConnected { get { return (m_client.GetServer().State == MongoServerState.Connected); } }
        public String DbName { get { return m_dbName; } set { m_dbName = value; } }
        public String CollectionName { get { return m_collectionName; } set { m_collectionName = value; } }
        public MongoCollection<TEntity> Collection { get { return m_collection; } }
        #endregion

        #region Constructors
        public Internet(string ip, string dbName, string collectionName)
        {
            m_ip = ip;
            CombineConnString(ip);
            m_dbName = dbName;
            m_collectionName = collectionName;
            m_client = new MongoClient(m_connectionStr);
            MongoServer server = m_client.GetServer();
            MongoDatabase database = server.GetDatabase(m_dbName);
            m_collection = database.GetCollection<TEntity>(m_collectionName);
        }
        #endregion

        #region Windows Form Events
        #endregion

        #region Public Methods
        public bool Connect()
        {
            try
            {
                m_collection.FindOne();
            }
            catch (MongoConnectionException e)
            {
                Console.WriteLine(m_client.GetServer().State.ToString());
                return false;
            }
            return true;
        }

        public void UpdateStringField(string auctionId, Expression<Func<AuctionEntity, string>> expression, string value)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            IMongoUpdate update = Update<AuctionEntity>.Set(expression, value);
            WriteConcernResult wcr = m_collection.Update(query, update);
            // TODO
        }

        public void UpdateStringField(string auctionId, AuctionColumnHeader columnType, string value)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            Expression<Func<AuctionEntity, string>> expression = e => e.Name;
            switch (columnType)
            {
                case AuctionColumnHeader.拍品名稱:
                    expression = e => e.Name;
                    break;
                case AuctionColumnHeader.庫存狀態:
                    expression = e => e.StockState;
                    break;
                default:
                    break;
            }
            IMongoUpdate update = Update<AuctionEntity>.Set(expression, value);
            WriteConcernResult wcr = m_collection.Update(query, update);
            // TODO
        }

        public void UpdateIntField(string auctionId, Expression<Func<AuctionEntity, int>> expression, int value)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            IMongoUpdate update = Update<AuctionEntity>.Set(expression, value);
            WriteConcernResult wcr = m_collection.Update(query, update);
            // TODO
        }

        public void UpdateIntField(string auctionId, AuctionColumnHeader columnType, int value)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            Expression<Func<AuctionEntity, int>> expression = e => e.ReturnState;
            switch (columnType)
            {
                case AuctionColumnHeader.歸還狀態:
                    expression = e => e.ReturnState;
                    break;
                case AuctionColumnHeader.買家適用服務費:
                    expression = e => e.BuyerServiceCharge;
                    break;
                case AuctionColumnHeader.保證金繳納:
                    expression = e => e.PayGuaranteeState;
                    break;
                case AuctionColumnHeader.保證金繳納金額:
                    expression = e => e.PayGuaranteeNumber;
                    break;
                case AuctionColumnHeader.保證金退還:
                    expression = e => e.ReturnGuaranteeState;
                    break;
                case AuctionColumnHeader.保證金退還金額:
                    expression = e => e.ReturnGuaranteeNumber;
                    break;
                case AuctionColumnHeader.付款方式:
                    expression = e => e.PayWayState;
                    break;
                default:
                    break;
            }
            IMongoUpdate update = Update<AuctionEntity>.Set(expression, value);
            WriteConcernResult wcr = m_collection.Update(query, update);
        }

        public void UpdateFloatField(string auctionId, AuctionColumnHeader columnType, float value)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            AuctionEntity entity = m_collection.FindOne(query) as AuctionEntity;
            Expression<Func<AuctionEntity, float>> expression = e => e.ReturnState;
            switch (columnType)
            {
                case AuctionColumnHeader.歸還狀態:
                    expression = e => e.ReturnState;
                    break;
                default:
                    break;
            }
            IMongoUpdate update = Update<AuctionEntity>.Set(expression, value);
            WriteConcernResult wcr = m_collection.Update(query, update);
        }

        public void Insert(TEntity entity)
        {
            m_collection.Insert(entity);
            try
            {
                WriteConcernResult wcr = m_collection.Save(entity);
                if (!wcr.Ok)
                {
                    Console.WriteLine("[Internet] Insert error!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Internet] Insert error!" + e.ToString());
            }
        }

        public void Remove(string auctionId)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.AuctionId, auctionId);
            m_collection.Remove(query);
        }

        public List<TEntity> GetCollectionList()
        {
            if (m_collectionList == null)
                m_collectionList = m_collection.FindAll().ToList<TEntity>();

            return m_collectionList;
        }

        public List<TEntity> SearchAuctions(int bidderId)
        {
            IMongoQuery query = Query<AuctionEntity>.EQ(e => e.BidderNumber, bidderId.ToString());
            List<TEntity> auctions = m_collection.Find(query).ToList<TEntity>();
            return auctions;
        }

        public TEntity GetBidderData(int bidderId)
        {
            IMongoQuery query = Query<BidderEntity>.EQ(e => e.BidderID, bidderId.ToString());
            TEntity bidder = m_collection.FindOne(query);
            return bidder;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void CombineConnString(string ip)
        {
            m_connectionStr = "mongodb://" + ip + ":" + m_port;
        }
        #endregion
    }
}
