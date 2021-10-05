using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TruckAppMVC.Config;
using TruckAppMVC.DTO;
using TruckAppMVC.Model;

namespace TruckAppMVC.Service
{
    public class TruckAppService : ITruckAppServiceInterface
    {
        //proper connection requirement
        public readonly MongoClient mongoClient = null;
        public readonly IMongoDatabase mongoDatabase = null;

        // Tables as documents (* Add Documents here)
        public readonly IMongoCollection<FaceBookTracker> facebookTracer = null;
        public readonly IMongoCollection<TruckShipments> truckShipments = null;
        public readonly IMongoCollection<StageOfShipment> stageOfShipment = null;
        public readonly IWebHostEnvironment webHostEnvironment;

        //configuration options
        private readonly IOptions<MyDbConfig> config;

        //Inject the Dependency Injection
        public TruckAppService(IOptions<MyDbConfig> _config, IWebHostEnvironment _webHostEnvironment)
        {
            try
            {
                config = _config;
                webHostEnvironment = _webHostEnvironment;
                mongoClient = new MongoClient(config.Value.ConnectionString);
                mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
                facebookTracer = mongoDatabase.GetCollection<FaceBookTracker>(config.Value.FBT_Collection);
                truckShipments = mongoDatabase.GetCollection<TruckShipments>(config.Value.TS_Collection);
                stageOfShipment = mongoDatabase.GetCollection<StageOfShipment>(config.Value.SOS_Collection);
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }      
        }

        // VALIDATING REFERENCENO
        public FaceBookTracker validatingReferenceNo(string referenceNo)
        {
            return facebookTracer.Find(rfn => rfn.ReferenceNo == referenceNo).FirstOrDefault();
        }



        // tab checking for stages. when we click on stage tab like collection, delivery, others. 
        public StageOfShipment StageTabChecking(string reference, string stage)
        {
            return stageOfShipment.Find(sc => sc.ReferenceNo == reference && sc.Stage == stage && sc.IsCompleted == "on").FirstOrDefault();
        }



        // Submitting stage to database ( --- IsCompleted and Comments For Stage --- )
        public StageOfShipment addingStageToDb(StageOfShipmentDTO stageOfShipmentDTO)
        {
            StageOfShipment stageOfShipmentDb = null;

            var data = stageOfShipment.Find(sv => sv.ReferenceNo == stageOfShipmentDTO.ReferenceNo &&
                                 sv.Stage == stageOfShipmentDTO.Stage).FirstOrDefault();

            if (data == null)
            {
                stageOfShipmentDb = new StageOfShipment();
                stageOfShipmentDb.ReferenceNo = stageOfShipmentDTO.ReferenceNo;
                stageOfShipmentDb.Stage = stageOfShipmentDTO.Stage;
                stageOfShipmentDb.IsCompleted = stageOfShipmentDTO.IsCompleted;
                stageOfShipmentDb.Comments = stageOfShipmentDTO.Comments;

                //inserting the values into dto to model and database
                stageOfShipment.InsertOne(stageOfShipmentDb);
            }
            else if (data != null)
            {
                stageOfShipmentDTO._id = data._id;
                //stageOfShipmentDb = new StageOfShipment();
                data.ReferenceNo = stageOfShipmentDTO.ReferenceNo;
                data.Stage = stageOfShipmentDTO.Stage;
                data.IsCompleted = stageOfShipmentDTO.IsCompleted;
                data.Comments = stageOfShipmentDTO.Comments;
                // replacing it after IsCompleted Check
                stageOfShipment.ReplaceOne(uid => uid._id == stageOfShipmentDTO._id, data);
            }
            return data;
        }



        // checking sub stage... Each stage inside we're having substage to check if data inside exists or not
        public TruckShipments checkingSubStage(TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart, TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            return truckShipments.Find(td => td.ReferenceNo == truckShipmentsDTOMultiPart.ReferenceNo && td.Stage == truckShipmentsDTOMultiPart.Stage &&
                                       td.ProcessName == truckShipmentsDTOMultiPart.ProcessName ||
                                       td.ReferenceNo == truckShipmentDTOBaseImg.ReferenceNo && td.Stage == truckShipmentDTOBaseImg.Stage &&
                                       td.ProcessName == truckShipmentDTOBaseImg.ProcessName).FirstOrDefault();
        }



        // take - a - photo 
        public TruckShipments cmrPostCollectionFromActionToBase64(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            TruckShipments truckShipmentsDb = null;

            var data = truckShipments.Find(tsc => tsc.ReferenceNo == truckShipmentDTOBaseImg.ReferenceNo &&
                                tsc.Stage == truckShipmentDTOBaseImg.Stage &&
                                tsc.ProcessName == truckShipmentDTOBaseImg.ProcessName).FirstOrDefault();

            if (data == null)
            {
                truckShipmentsDb = new TruckShipments();
                truckShipmentsDb.ReferenceNo = truckShipmentDTOBaseImg.ReferenceNo;
                truckShipmentsDb.Stage = truckShipmentDTOBaseImg.Stage;
                truckShipmentsDb.ProcessName = truckShipmentDTOBaseImg.ProcessName;
                truckShipmentsDb.Image = truckShipmentDTOBaseImg.File;

                truckShipments.InsertOne(truckShipmentsDb);
            }
            else if (data != null)
            {
                return truckShipmentsDb;
            }
            return truckShipmentsDb;
        }



        // select - a - photo
        public TruckShipments cmrPostCollectionFromActionToMultiPart(TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            TruckShipments truckShipmentsDb = null;

            var data = truckShipments.Find(tsc => tsc.ReferenceNo == truckShipmentsDTO.ReferenceNo &&
                                tsc.Stage == truckShipmentsDTO.Stage &&
                                tsc.ProcessName == truckShipmentsDTO.ProcessName).FirstOrDefault();

            if (data == null)
            {
                var file = string.Empty;
                if (truckShipmentsDTO.ImageName.Length > 0)
                {
                    try
                    {
                        string path = webHostEnvironment.ContentRootPath + "\\File\\";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        using (FileStream fs = System.IO.File.Create(path + truckShipmentsDTO.ImageName.FileName))
                        {
                            truckShipmentsDTO.ImageName.CopyTo(fs);
                            fs.Flush();
                            file = webHostEnvironment.ContentRootPath + "\\" + truckShipmentsDTO.ImageName.FileName;
                        }
                    }
                    catch (Exception e)
                    {
                        e.StackTrace.ToString();
                    }
                }
                //getting dto object into db object
                truckShipmentsDb = new TruckShipments();
                truckShipmentsDb.ReferenceNo = truckShipmentsDTO.ReferenceNo;
                truckShipmentsDb.Stage = truckShipmentsDTO.Stage;
                truckShipmentsDb.ProcessName = truckShipmentsDTO.ProcessName;
                truckShipmentsDb.Image = truckShipmentsDTO.Image;

                //creating the data inside the database
                truckShipments.InsertOne(truckShipmentsDb);
            }
            else if (data != null)
            {
                return data;
            }
            return data;
        }



        //collection - others
        public TruckShipments stageInsideOthersFileName(OthersDTO othersDTO)
        {
            return truckShipments.Find(oth => oth.ReferenceNo == othersDTO.ReferenceNo && oth.Stage == othersDTO.StageName &&
                                       oth.ProcessName == othersDTO.ProcessName).FirstOrDefault();
        }


    }
}