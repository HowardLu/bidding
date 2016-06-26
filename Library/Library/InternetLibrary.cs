using System;
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
        public string BidderNumber { get; set; }
        public string StockState { get; set; }
        public int ReturnState { get; set; }
        public int HammerPrice { get; set; }
        public int BuyerServiceCharge { get; set; }
        public int FinalPrice { get; set; }
        public int ReturnGuaranteeState { get; set; }
        public int ReturnGuaranteeNumber { get; set; }
        public int PayWayState { get; set; }
        public int SellerServiceCharge { get; set; }
        public int SellerAccountPayable { get; set; }
        public string PhotoUrl { get; set; }
        public string Artist { get; set; }
        public string Artwork { get; set; }
        public int InitialPrice { get; set; }
        public int NowPrice { get; set; }
        public string Auctioneer { get; set; }
        public int CheckoutNumber { get; set; }
        public string CheckoutTime { get; set; }

        public AuctionEntity()
        {
            Id = ObjectId.Empty;
            AuctionId = BidderNumber = StockState = PhotoUrl = Artist = Artwork = Auctioneer = CheckoutTime = "";
            ReturnState = HammerPrice = BuyerServiceCharge = FinalPrice = ReturnGuaranteeState = ReturnGuaranteeNumber = PayWayState =
                SellerServiceCharge = SellerAccountPayable = InitialPrice = NowPrice = CheckoutNumber = 0;
        }
    }

    public class BidderEntity
    {
        public int _id { get; set; }
        public string BankContact { get; set; }
        public string Fax { get; set; }
        public int BidderID_int { get; set; }
        public string Tel { get; set; }
        public string Name { get; set; }
        public string GuaranteeCost { get; set; }
        public string Company { get; set; }
        public string GuaranteeType { get; set; }
        public string BankAcc { get; set; }
        public string CareerTitle { get; set; }
        public string IDNumber { get; set; }
        public string ServiceFee { get; set; }
        public string BankContactTel { get; set; }
        public string Address { get; set; }
        public string BidderID { get; set; }
        public string CreditCardID { get; set; }
        public string Auctioneer { get; set; }
        public string CreditCardType { get; set; }
        public string EMail { get; set; }
        public string Bank { get; set; }

        public BidderEntity()
        {
            _id = BidderID_int = 0;
            Fax = Tel = Name = GuaranteeCost = Company = GuaranteeType = BankAcc = CareerTitle = IDNumber = BankContactTel =
                Address = BidderID = CreditCardID = Auctioneer = CreditCardType = EMail = Bank = "";
            GuaranteeCost = "0";
            ServiceFee = "20";
            GuaranteeType = BiddingLibrary.PayGuarantee.台幣現鈔.ToString();
        }
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

    public class DealerItemEntity
    {
        public string _id { get; set; }
        public string ReservePrice { get; set; }
        public string BuildImg { get; set; }
        public string SrcDealer { get; set; }
        public string ItemPS { get; set; }
        public string Remain { get; set; }
        public string ItemNum { get; set; }
        public string LotNO { get; set; }
        public string PackImg { get; set; }
        public string ItemName { get; set; }
        public string Spec { get; set; }

        public DealerItemEntity()
        {
            _id = ReservePrice = /*BuildImg =*/ SrcDealer = ItemPS = Remain = ItemNum = LotNO = /*PackImg =*/ ItemName = Spec = "";
        }
    }

    public class DealerEntity
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
        public string ContractDate { get; set; }
        public string CellPhone { get; set; }

        public DealerEntity()
        {
            _id = IfDealedInsuranceFee = FrameFee = Fax = FireFee = Tel = Name = BankName = IdentifyFee = Country =
                Address = IfDealedPictureFee = BankAcc = IfNDealedPictureFee = IfNDealedInsuranceFee =
                CardID = IfDealedServiceFee = PostID = IfNDealedServiceFee = ContractID = "";
        }

        public void parseDealedFee(out int pictureFee, out double insuranceFeeP, out double serviceFeeP)
        {
            pictureFee = 0;
            insuranceFeeP = 0.0;
            serviceFeeP = 0.0;

            int.TryParse(IfDealedPictureFee, out pictureFee);
            double.TryParse(IfDealedInsuranceFee, out insuranceFeeP);
            double.TryParse(IfDealedServiceFee, out serviceFeeP);
        }

        public void parseOtherFee(out int otherFee)
        {
            int fireFee, frameFee, identifyFee;
            fireFee = frameFee = identifyFee = 0;

            int.TryParse(FireFee, out fireFee);
            int.TryParse(FrameFee, out frameFee);
            int.TryParse(IdentifyFee, out identifyFee);

            otherFee = fireFee + frameFee + identifyFee;
        }
    }

    public class MemberEntity
    {
        public ObjectId _id { get; set; }
        public string Account { get; set; }
    }

    public enum AuctionColumnHeader
    {
        流水號 = 0,
        拍品編號,
        拍品名稱,
        尺寸,
        得標牌號,
        買家姓名,
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
        應付賣家金額,
        Count
    }

    /*public class AuctionEntityTW
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
    }*/

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

        ~Internet()
        {
            m_client = null;
            m_collection = null;
            if (null != m_collectionList)
                m_collectionList = null;
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
                Console.WriteLine(m_client.GetServer().State.ToString() + "\n" + e.ToString());
                System.Windows.Forms.MessageBox.Show("連線失敗!\n\n" + e.ToString());
                return false;
            }
            return true;
        }

        public void UpdateField<TKey, TValue>(Expression<Func<TEntity, TKey>> keyExpression, TKey key,
            Expression<Func<TEntity, TValue>> valueExpression, TValue value)
        {
            IMongoQuery query = Query<TEntity>.EQ(keyExpression, key);
            IMongoUpdate update = Update<TEntity>.Set(valueExpression, value);
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

        public void Remove<TKey>(Expression<Func<TEntity, TKey>> keyExpression, TKey key)
        {
            IMongoQuery query = Query<TEntity>.EQ(keyExpression, key);
            m_collection.Remove(query);
        }

        public void RemoveOne<TKey>(Expression<Func<TEntity, TKey>> keyExpression, TKey key)
        {
            IMongoQuery query = Query<TEntity>.EQ(keyExpression, key);
            m_collection.Remove(query, RemoveFlags.Single);
        }

        public List<TEntity> GetCollectionList()
        {
            if (null == m_collectionList || 0 == m_collectionList.Count)
            {
                m_collectionList = m_collection.FindAll().ToList<TEntity>();
            }
            return m_collectionList;
        }

        public void ClearCollectionList()
        {
            if (null != m_collectionList)
            {
                m_collectionList.Clear();
            }
        }

        public List<TEntity> Find<TKey>(Expression<Func<TEntity, TKey>> keyExpression, TKey key)
        {
            IMongoQuery query = Query<TEntity>.EQ(keyExpression, key);
            List<TEntity> entities = m_collection.Find(query).ToList<TEntity>();
            return entities;
        }

        public TEntity FineOne<TKey>(Expression<Func<TEntity, TKey>> expression, TKey value)
        {
            IMongoQuery query = Query<TEntity>.EQ(expression, value);
            TEntity entity = m_collection.FindOne(query);
            return entity;
        }

        public TEntity Max<TKey>(Expression<Func<TEntity, TKey>> keyExpression, TKey key, string fieldName)
        {
            IMongoQuery query = Query<TEntity>.EQ(keyExpression, key);
            MongoCursor<TEntity> entity = m_collection.Find(query).SetSortOrder(SortBy.Descending(fieldName));
            return entity.First<TEntity>();
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
