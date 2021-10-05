using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TruckAppMVC.Common;
using TruckAppMVC.Config;
using TruckAppMVC.DTO;
using TruckAppMVC.Model;
using TruckAppMVC.Service;
using static System.Net.Mime.MediaTypeNames;

namespace TruckAppMVC.Controllers
{
    public class TruckappController : Controller
    {

        private readonly ITruckAppServiceInterface truckAppServiceInterface;
        

        public TruckappController(ITruckAppServiceInterface _truckAppServiceInterface)
        {
            truckAppServiceInterface = _truckAppServiceInterface;
        }


        public IActionResult IndexPage()
        {
            return View();
        }


        [Route("/login")]
        public IActionResult login()
        {
            return View();
        }

        // for getting this refNo to show on home page
        private static string ReferenceNo = null;
        private static string OthersPicName = null;

        //Login Authentication
        [Route("/auth")]
        public ActionResult validation(string referenceNo)
        {
            var data = truckAppServiceInterface.validatingReferenceNo(referenceNo);
            if (data != null)
            {
                ReferenceNo = data.ReferenceNo;
                ViewBag.DelDate = data.DelDate.ToString();
                ViewBag.ReferenceNo = data.ReferenceNo.ToString();
                return View("loginSuc");
            }
            else
            {
                return View("loginErr");
            }
        }




        [Route("/home")]
        public ActionResult home()
        {
            ViewBag.ReferenceNo = ReferenceNo;
            return View();
        }

//-------------------------------------------------- COLLECTION ----------------------------------------------------------------
        [Route("/stage/Collection")]
        public IActionResult Collection()
        {
            return View();
        }


        // checing collection tab
        [Route("/CheckStage/Collection")]
        public IActionResult checkingTab()
        {
            var referenceNo = ReferenceNo;
            var stage = StageNames.Collection;
            var IsCompletedIfExists = truckAppServiceInterface.StageTabChecking(referenceNo, stage);
            if (IsCompletedIfExists == null)
            {
                return View("Collection");
            }
            else if (IsCompletedIfExists.IsCompleted == "on" && IsCompletedIfExists != null)
            {
                return View("StageAlert");
            }
            return View("Collection");
        }


        //if collection not submitted then process
        [Route("/Collection")]
        public IActionResult collectionStageValidation(StageOfShipmentDTO stageOfShipmentDTO)
        {
            stageOfShipmentDTO.ReferenceNo = ReferenceNo;
            stageOfShipmentDTO.Stage = StageNames.Collection;
            ViewBag.StageName = StageNames.Collection;
            var data = truckAppServiceInterface.addingStageToDb(stageOfShipmentDTO);
            if (data != null)
            {
                return View("StageSuccess");
            }
            return View("StageSuccess");
        }


        //checking sub stage for cmr [collection - CMR]
        [HttpGet]
        [Route("/checkSubStage/CMR")]
        public IActionResult checkingSubStage()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Collection;
            truckShipmentsDTOMultiPart.ProcessName = CollectionProcess.CMR_Post_Collection;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.CMR_Post_Collection;

            ViewBag.SubStageName = CollectionProcess.CMR_Post_Collection;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CMRPostCollection");
            }
            else if (data != null)
            {
                return View("CollectionSubStageError");
            }
            return View("CMRPostCollection");
        }


        // view sub-stage collection - cmr
        [Route("/CMR-Post-Collection")]
        public IActionResult CMRPostCollection()
        {
            return View();
        }


        // collection - cmr - select a photo
        [HttpPost]
        [Route("/CMR-Post-Collection/select-photo")]
        public IActionResult stageToDBTakePhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Collection;
            truckShipmentsDTO.ProcessName = CollectionProcess.CMR_Post_Collection;

            ViewBag.ProcessName = CollectionProcess.CMR_Post_Collection;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        // collection - cmr - take a photo
        [HttpPost]
        [Route("/CMR-Post-Collection/take-photo")]
        public IActionResult stageToDBSelectPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.CMR_Post_Collection;
            ViewBag.ProcessName = CollectionProcess.CMR_Post_Collection;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        // collection - Photo seal on lock
        [HttpGet]
        [Route("/checkSubStage/PSOL")]
        public IActionResult collection_checkingSubStage_PSOL()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Collection;
            truckShipmentsDTOMultiPart.ProcessName = CollectionProcess.Photo_Seal_On_Lock;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Photo_Seal_On_Lock;

            ViewBag.ProcessName = CollectionProcess.Photo_Seal_On_Lock;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionPhotoSealOnLock");
            }
            else if (data != null)
            {
                return View("CollectionSubStageError");
            }
            return View("CollectionPhotoSealOnLock");
        }



        // collection - psol - select a photo
        [HttpPost]
        [Route("/Photo-Seal-On-Lock/select-photo")]
        public IActionResult collection_PhotoSealOnLock_SelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Collection;
            truckShipmentsDTO.ProcessName = CollectionProcess.Photo_Seal_On_Lock;

            ViewBag.ProcessName = CollectionProcess.Photo_Seal_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        //collection - pson - take a photo
        [HttpPost]
        [Route("/Photo-Seal-On-Lock/take-photo")]
        public IActionResult collection_PhotoSealOnLock_TakePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Photo_Seal_On_Lock;
            ViewBag.ProcessName = CollectionProcess.Photo_Seal_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        [HttpGet]
        [Route("/checkSUbStage/PSOT")]
        public IActionResult collection_checkingSubStage_PSOT()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Collection;
            truckShipmentsDTOMultiPart.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;

            ViewBag.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionPhotoSealOnTrailer");
            }
            else if (data != null)
            {
                return View("CollectionSubStageError");
            }
            return View("CollectionPhotoSealOnTrailer");
        }


        [HttpPost]
        [Route("/Photo-Seal-On-Trailer/select-photo")]
        public IActionResult collection_PhotoSealOnTrailer_SelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Collection;
            truckShipmentsDTO.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;

            ViewBag.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        [HttpPost]
        [Route("/Photo-Seal-On-Trailer/take-photo")]
        public IActionResult collection_PhotoSealOnTrailer_TakePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;
            ViewBag.ProcessName = CollectionProcess.Photo_Seal_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }



        [HttpGet]
        [Route("/checkStage/PIST")]
        public IActionResult collection_checkingSubStage_PSIT()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Collection;
            truckShipmentsDTOMultiPart.ProcessName = CollectionProcess.Photo_Inside_Trailer;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Photo_Inside_Trailer;

            ViewBag.ProcessName = CollectionProcess.Photo_Inside_Trailer;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionPhotoInsideTrailer");
            }
            else if (data != null)
            {
                return View("CollectionSubStageError");
            }
            return View("CollectionPhotoInsideTrailer");
        }


        [HttpPost]
        [Route("/Photo-Inside-Trailer/select-photo")]
        public IActionResult collection_PhotoInsideTrailer_SelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Collection;
            truckShipmentsDTO.ProcessName = CollectionProcess.Photo_Inside_Trailer;

            ViewBag.ProcessName = CollectionProcess.Photo_Inside_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        [HttpPost]
        [Route("/Photo-Inside-Trailer/take-photo")]
        public IActionResult collection_PhotoInsideTrailer_TakePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Photo_Inside_Trailer;
            ViewBag.ProcessName = CollectionProcess.Photo_Inside_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }

        [HttpPost]
        [Route("/checkStage/others")]
        public IActionResult collection_others_otherPic(string otherPic)
        {
            string special = "---";
            OthersDTO othersDTO = new OthersDTO();
            othersDTO.ReferenceNo = ReferenceNo;
            othersDTO.StageName = StageNames.Collection;
            othersDTO.ProcessName = string.Concat(CollectionProcess.Others.ToString(), special, otherPic);
            othersDTO.Image = null;

            ViewBag.ProcessName = otherPic;
            OthersPicName = otherPic;

            var data = truckAppServiceInterface.stageInsideOthersFileName(othersDTO);
            if (data == null)
            {
                return View("CollectionOthersProcess");
            }
            return View("CollectionSubStageError");
        }



        [HttpPost]
        [Route("/collection/others/select-photo")]
        public IActionResult collection_Others_selectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            string special = "---";
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Collection;
            truckShipmentsDTO.ProcessName = CollectionProcess.Others + special + OthersPicName;

            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }


        [HttpPost]
        [Route("/collection/others/take-photo")]
        public IActionResult collection_Others_takePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            string special = "---";
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collection;
            truckShipmentDTOBaseImg.ProcessName = CollectionProcess.Others + special + OthersPicName; ;
            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectionSubStageSuccess");
            }
            return View("CollectionSubStageSuccess");
        }

        //-------------------------------------------- CUSTOMS ------------------------------------------------------------------------ 


        [Route("/stage/Customs")]
        public IActionResult redirectingToCustomsPage()
        {
            return View("Customs");
        }


        //customs page
        [Route("/CheckStage/Customs")]
        public IActionResult checkingTabForCustoms()
        {
            var referenceNo = ReferenceNo;
            var stage = StageNames.Customs;
            var IsCompletedIfExists = truckAppServiceInterface.StageTabChecking(referenceNo, stage);
            if (IsCompletedIfExists == null)
            {
                ViewBag.StageName = StageNames.Customs;
                return View("Customs");
            }
            else if (IsCompletedIfExists.IsCompleted == "on" && IsCompletedIfExists != null)
            {
                return View("StageAlert");
            }
            return View("Customs");
        }


        // When CheckBox is checked this method will be invkoed to store it in database
        [HttpPost]
        [Route("/Customs")]
        public IActionResult chekingCustomsTab(StageOfShipmentDTO stageOfShipmentDTO)
        {
            stageOfShipmentDTO.ReferenceNo = ReferenceNo;
            stageOfShipmentDTO.Stage = StageNames.Customs;
            ViewBag.StageName = StageNames.Customs;
            var data = truckAppServiceInterface.addingStageToDb(stageOfShipmentDTO);
            if (data != null)
            {
                return View("StageSuccess");
            }
            return View("StageSuccess");
        }


        // customs - PNSOL - substage checking
        [HttpGet]
        [Route("/checkSubStage/PNSOL")]
        public IActionResult customs_photoNewSealsOnLock_subStage()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Customs;
            truckShipmentsDTOMultiPart.ProcessName = CustomsProcess.Photo_New_Seals_On_Lock;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Photo_New_Seals_On_Lock;

            ViewBag.SubStageName = CustomsProcess.Photo_New_Seals_On_Lock;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("Customs_PhotoNewSealOnLock");
            }
            else if (data != null)
            {
                return View("CustomsSubStageError");
            }
            return View("Customs_PhotoNewSealOnLock");
        }


        // customs - PNSOL - take a photo
        [HttpPost]
        [Route("/Photo-New-Seals-On-Lock/take-photo")]
        public IActionResult customs_PNSOL_takeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Photo_New_Seals_On_Lock;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.Photo_New_Seals_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


        // customs - PNSOL - select a photo
        [HttpPost]
        [Route("/Photo-New-Seals-On-Lock/select-photo")]
        public IActionResult customs_PNSOL_selectAPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Customs;
            truckShipmentsDTO.ProcessName = CustomsProcess.Photo_New_Seals_On_Lock;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.Photo_New_Seals_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }



        // customs - PNSOT - substage checking
        [HttpGet]
        [Route("/checkSubStage/PNSOT")]
        public IActionResult customs_photoNewSealsOnTrailer_subStage()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Customs;
            truckShipmentsDTOMultiPart.ProcessName = CustomsProcess.Photo_New_Seals_On_Trailer;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Photo_New_Seals_On_Trailer;

            ViewBag.SubStageName = CustomsProcess.Photo_New_Seals_On_Trailer;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("Customs_PhotoNewSealOnTrailer");
            }
            else if (data != null)
            {
                return View("CustomsSubStageError");
            }
            return View("Customs_PhotoNewSealOnTrailer");
        }


        // customs - PNSOT - take a photo
        [HttpPost]
        [Route("/Photo-New-Seals-On-Trailer/take-photo")]
        public IActionResult customs_PNSOT_takeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Photo_New_Seals_On_Trailer;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.Photo_New_Seals_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }



        // customs - PNSOT - select a photo
        [HttpPost]
        [Route("/Photo-New-Seals-On-Trailer/select-photo")]
        public IActionResult customs_PNSOT_selectAPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Customs;
            truckShipmentsDTO.ProcessName = CustomsProcess.Photo_New_Seals_On_Trailer;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.Photo_New_Seals_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


        // customs - PCOST - substage checking
        [HttpGet]
        [Route("/checkSubStage/PCOS")]
        public IActionResult customs_photoCustomsOnStamp_subStage()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Customs;
            truckShipmentsDTOMultiPart.ProcessName = CustomsProcess.Photo_Customs_Stamp;
            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Photo_Customs_Stamp;

            ViewBag.SubStageName = CustomsProcess.Photo_Customs_Stamp;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("Customs_PhotoCustomsOnStamp");
            }
            else if (data != null)
            {
                return View("CustomsSubStageError");
            }
            return View("Customs_PhotoCustomsOnStamp");
        }


        // customs - PCOST - take a photo
        [HttpPost]
        [Route("/Photo-CustomsStamp/take-photo")]
        public IActionResult customs_PCOST_takeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Photo_Customs_Stamp;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.Photo_Customs_Stamp;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


        // customs - PCOST - select a photo
        [HttpPost]
        [Route("/Photo-CustomsStamp/select-photo")]
        public IActionResult customs_PCOST_selectAPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Customs;
            truckShipmentsDTO.ProcessName = CustomsProcess.Photo_Customs_Stamp;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.Photo_Customs_Stamp;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


        // customs - CMR-PC - substageChecking
        [HttpGet]
        [Route("/checkSubStage/CMR-PC")]
        public IActionResult customs_cmrPostCustoms_subStage()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Customs;
            truckShipmentsDTOMultiPart.ProcessName = CustomsProcess.CMR_Post_Customs;
            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.CMR_Post_Customs;

            ViewBag.SubStageName = CustomsProcess.CMR_Post_Customs;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("Customs_CMR-PostCustoms");
            }
            else if (data != null)
            {
                return View("CustomsSubStageError");
            }
            return View("Customs_PhotoCustomsOnStamp");
        }



        // customs - CMR-PC - take a photo
        [HttpPost]
        [Route("/CMR-PostCustoms/take-photo")]
        public IActionResult customs_cmrPostCustoms_takeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.CMR_Post_Customs;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.CMR_Post_Customs;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }



        // customs - CMR-PC - select a photo
        [HttpPost]
        [Route("/CMR-PostCustoms/select-photo")]
        public IActionResult customs_cmrPostCustoms_selectAPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Customs;
            truckShipmentsDTO.ProcessName = CustomsProcess.CMR_Post_Customs;

            ViewBag.StageName = StageNames.Customs;
            ViewBag.ProcessName = CustomsProcess.CMR_Post_Customs;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


        //customs - checkstage - other - Filename
        [HttpPost]
        [Route("/customs/checkStage/others")]
        public IActionResult customs_others_otherPic(string otherPic)
        {
            string special = "---";
            OthersDTO othersDTO = new OthersDTO();
            othersDTO.ReferenceNo = ReferenceNo;
            othersDTO.StageName = StageNames.Customs;
            othersDTO.ProcessName = string.Concat(CustomsProcess.Others.ToString(), special, otherPic);
            othersDTO.Image = null;

            ViewBag.ProcessName = otherPic;
            OthersPicName = otherPic;

            var data = truckAppServiceInterface.stageInsideOthersFileName(othersDTO);
            if (data == null)
            {
                return View("CustomsOthersProcess");
            }
            return View("CustomsSubStageError");
        }


        //customs - checkstage - other - select a photo
        [HttpPost]
        [Route("/customs/others/select-photo")]
        public IActionResult customs_Others_selectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            string special = "---";
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Customs;
            truckShipmentsDTO.ProcessName = CustomsProcess.Others + special + OthersPicName;

            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


        //customs - checkstage - other - take a photo
        [HttpPost]
        [Route("/customs/others/take-photo")]
        public IActionResult customs_Others_takePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            string special = "---";
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Customs;
            truckShipmentDTOBaseImg.ProcessName = CustomsProcess.Others + special + OthersPicName; ;
            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CustomsSubStageSuccess");
            }
            return View("CustomsSubStageSuccess");
        }


//----------------------------------------------- DELIVERY --------------------------------------------------------

        [Route("/stage/Delivery")]
        public IActionResult Delivery()
        {
            return View("Delivery");
        }


        [Route("/CheckStage/Delivery")]
        public IActionResult checkingTabForDelivery()
        {
            var referenceNo = ReferenceNo;
            var stage = StageNames.Delivery;
            var IsCompletedIfExists = truckAppServiceInterface.StageTabChecking(referenceNo, stage);
            if (IsCompletedIfExists == null)
            {
                ViewBag.StageName = StageNames.Delivery;
                return View("Delivery");
            }
            else if (IsCompletedIfExists.IsCompleted == "on" && IsCompletedIfExists != null)
            {
                return View("StageAlert");
            }
            return View("Delivery");
        }


        // When CheckBox is checked this method will be invkoed to store it in database
        [HttpPost]
        [Route("/Delivery")]
        public IActionResult chekingDeliveryTab(StageOfShipmentDTO stageOfShipmentDTO)
        {
            stageOfShipmentDTO.ReferenceNo = ReferenceNo;
            stageOfShipmentDTO.Stage = StageNames.Delivery;
            ViewBag.StageName = StageNames.Delivery;
            var data = truckAppServiceInterface.addingStageToDb(stageOfShipmentDTO);
            if (data != null)
            {
                return View("StageSuccess");
            }
            return View("StageSuccess");
        }


        // Delivery - CMR-Post Delivery - substage checking
        [HttpGet]
        [Route("/checkSubStage/CMR-PD")]
        public IActionResult delivery_cmrPostDelivery_subStage()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Delivery;
            truckShipmentsDTOMultiPart.ProcessName = DeliveryProcess.CMR_Post_Delivery;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Delivery;
            truckShipmentDTOBaseImg.ProcessName = DeliveryProcess.CMR_Post_Delivery;

            ViewBag.SubStageName = DeliveryProcess.CMR_Post_Delivery;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("Delivery_CMR_PostDelivery");
            }
            else if (data != null)
            {
                return View("DeliverySubStageError");
            }
            return View("Delivery_CMR_PostDelivery");
        }


        // Delivery - CMR-PD - take a photo
        [HttpPost]
        [Route("/CMR-PostDelivery/take-photo")]
        public IActionResult delivery_CMR_PD_takeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Delivery;
            truckShipmentDTOBaseImg.ProcessName = DeliveryProcess.CMR_Post_Delivery;

            ViewBag.StageName = StageNames.Delivery;
            ViewBag.ProcessName = DeliveryProcess.CMR_Post_Delivery;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("DeliverySubStageSuccess");
            }
            return View("DeliverySubStageSuccess");
        }


        // Delivery - CMR-PD - select a photo
        [HttpPost]
        [Route("/CMR-PostDelivery/select-photo")]
        public IActionResult delivery_CMR_PD_selectAPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Delivery;
            truckShipmentsDTO.ProcessName = DeliveryProcess.CMR_Post_Delivery;

            ViewBag.StageName = StageNames.Delivery;
            ViewBag.ProcessName = DeliveryProcess.CMR_Post_Delivery;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("DeliverySubStageSuccess");
            }
            return View("DeliverySubStageSuccess");
        }


        // delivery - checkstage - others
        [HttpPost]
        [Route("/delivery/checkStage/others")]
        public IActionResult delivery_others_otherPic(string otherPic)
        {
            string special = "---";
            OthersDTO othersDTO = new OthersDTO();
            othersDTO.ReferenceNo = ReferenceNo;
            othersDTO.StageName = StageNames.Delivery;
            othersDTO.ProcessName = string.Concat(DeliveryProcess.Others.ToString(), special, otherPic);
            othersDTO.Image = null;

            ViewBag.ProcessName = otherPic;
            OthersPicName = otherPic;

            var data = truckAppServiceInterface.stageInsideOthersFileName(othersDTO);
            if (data == null)
            {
                return View("DeliveryOthersProcess");
            }
            return View("DeliverySubStageError");
        }



        //delivery - checkstage - other - select a photo
        [HttpPost]
        [Route("/delivery/others/select-photo")]
        public IActionResult delivery_Others_selectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            string special = "---";
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Delivery;
            truckShipmentsDTO.ProcessName = DeliveryProcess.Others + special + OthersPicName;

            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("DeliverySubStageSuccess");
            }
            return View("DeliverySubStageSuccess");
        }


        //Delivery - checkstage - other - take a photo
        [HttpPost]
        [Route("/delivery/others/take-photo")]
        public IActionResult delivery_Others_takePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            string special = "---";
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Delivery;
            truckShipmentDTOBaseImg.ProcessName = DeliveryProcess.Others + special + OthersPicName; ;
            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("DeliverySubStageSuccess");
            }
            return View("DeliverySubStageSuccess");
        }


//------------------------------------------ COLLECTING THE EMPTIES ---------------------------------------------


        // Showing Stage page of Collecting the empties
        [Route("/Stage/CollectingTheEmpties")]
        public IActionResult CollectingTheEmpties()
        {
            return View("CollectingTheEmpties");
        }

        // checkingStage for collectingTheEmpties
        [Route("/CheckStage/Collecting-The-Empties")]
        public IActionResult checkingTabCollectingTheEmpties()
        {
            var referenceNo = ReferenceNo;
            var stage = StageNames.Collecting_The_Empties;
            var IsCompletedIfExists = truckAppServiceInterface.StageTabChecking(referenceNo, stage);
            if (IsCompletedIfExists == null)
            {
                ViewBag.StageName = StageNames.Collecting_The_Empties;
                return View("CollectingTheEmpties");
            }
            else if (IsCompletedIfExists.IsCompleted == "on" && IsCompletedIfExists != null)
            {
                return View("StageAlert");
            }
            return View("CollectingTheEmpties");
        }


        // When CheckBox is checked this method will be invkoed to store it in database
        [HttpPost]
        [Route("/Collecting-The-Empties")]
        public IActionResult chekingCTEmptiesTab(StageOfShipmentDTO stageOfShipmentDTO)
        {
            stageOfShipmentDTO.ReferenceNo = ReferenceNo;
            stageOfShipmentDTO.Stage = StageNames.Collecting_The_Empties;
            ViewBag.StageName = StageNames.Collecting_The_Empties;
            var data = truckAppServiceInterface.addingStageToDb(stageOfShipmentDTO);
            if (data != null)
            {
                return View("StageSuccess");
            }
            return View("StageSuccess");
        }


        // collectingTheEmpties - checkstage - others
        [HttpPost]
        [Route("/CollectingTheEmpties/checkStage/others")]
        public IActionResult collectingTheEmpties_others_otherPic(string otherPic)
        {
            string special = "---";
            OthersDTO othersDTO = new OthersDTO();
            othersDTO.ReferenceNo = ReferenceNo;
            othersDTO.StageName = StageNames.Collecting_The_Empties;
            othersDTO.ProcessName = string.Concat(CollectingTheEmptiesProcess.Others.ToString(), special, otherPic);
            othersDTO.Image = null;

            ViewBag.ProcessName = otherPic;
            OthersPicName = otherPic;

            var data = truckAppServiceInterface.stageInsideOthersFileName(othersDTO);
            if (data == null)
            {
                return View("CollectingTheEmptiesOthersProcess");
            }
            return View("CollectingTheEmptiesSubStageError");
        }


        //collectingTheEmpties - checkstage - other - select a photo
        [HttpPost]
        [Route("/collectingTheEmpties/others/select-photo")]
        public IActionResult ctEmpties_Others_selectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            string special = "---";
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Collecting_The_Empties;
            truckShipmentsDTO.ProcessName = CollectingTheEmptiesProcess.Others + special + OthersPicName;

            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("CollectingTheEmptiesSubStageSuccess");
            }
            return View("CollectingTheEmptiesSubStageSuccess");
        }


        //CollectingTheEmpties - checkstage - other - take a photo
        [HttpPost]
        [Route("/collectingTheEmpties/others/take-photo")]
        public IActionResult ctEmpties_Others_takePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            string special = "---";
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Collecting_The_Empties;
            truckShipmentDTOBaseImg.ProcessName = CollectingTheEmptiesProcess.Others + special + OthersPicName; ;
            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("CollectingTheEmptiesSubStageSuccess");
            }
            return View("CollectingTheEmptiesSubStageSuccess");
        }


//----------------------------------------------- OTHERS ---------------------------------------------------------


        [Route("/stage/Other")]
        public IActionResult Other()
        {
            return View("Other");
        }


        // checkingStage for Other
        [Route("/CheckStage/Other")]
        public IActionResult checkingTabOther()
        {
            var referenceNo = ReferenceNo;
            var stage = StageNames.Other;
            var IsCompletedIfExists = truckAppServiceInterface.StageTabChecking(referenceNo, stage);
            if (IsCompletedIfExists == null)
            {
                ViewBag.StageName = StageNames.Other;
                return View("Other");
            }
            else if (IsCompletedIfExists.IsCompleted == "on" && IsCompletedIfExists != null)
            {
                return View("StageAlert");
            }
            return View("Other");
        }


        // When CheckBox is checked this method will be invkoed to store it in database
        [HttpPost]
        [Route("/Other")]
        public IActionResult chekingOtherTab(StageOfShipmentDTO stageOfShipmentDTO)
        {
            stageOfShipmentDTO.ReferenceNo = ReferenceNo;
            stageOfShipmentDTO.Stage = StageNames.Other;
            ViewBag.StageName = StageNames.Other;
            var data = truckAppServiceInterface.addingStageToDb(stageOfShipmentDTO);
            if (data != null)
            {
                return View("StageSuccess");
            }
            return View("StageSuccess");
        }


        //checking sub stage for cmr [Other - CMR]
        [HttpGet]
        [Route("/Other/checkSubStage/CMR")]
        public IActionResult checkingSubStageForCMR()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.CMR_Post_Collection;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.CMR_Post_Collection;

            ViewBag.SubStageName = OtherProcess.CMR_Post_Collection;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherCMRPostCollection");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherCMRPostCollection");
        }


        // Other - cmr - select a photo
        [HttpPost]
        [Route("/Other/CMR/select-photo")]
        public IActionResult OtherCMRSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.CMR_Post_Collection;

            ViewBag.ProcessName = OtherProcess.CMR_Post_Collection;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - cmr - take a photo
        [HttpPost]
        [Route("/Other/CMR/take-photo")]
        public IActionResult OtherCMRTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.CMR_Post_Collection;
            ViewBag.ProcessName = OtherProcess.CMR_Post_Collection;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for PSOL [Other - PSOL]
        [HttpGet]
        [Route("/Other/checkSubStage/PSOL")]
        public IActionResult checkingSubStageForPSOL()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.Photo_Seal_On_Lock;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_Seal_On_Lock;

            ViewBag.SubStageName = OtherProcess.Photo_Seal_On_Lock;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherPhotoSealOnLock");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherPhotoSealOnLock");
        }


        // Other - PSOL - select a photo
        [HttpPost]
        [Route("/Other/PSOL/select-photo")]
        public IActionResult OtherPSOLSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Photo_Seal_On_Lock;

            ViewBag.ProcessName = OtherProcess.Photo_Seal_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - PSOL - take a photo
        [HttpPost]
        [Route("/Other/PSOL/take-photo")]
        public IActionResult OtherPSOLTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_Seal_On_Lock;
            ViewBag.ProcessName = OtherProcess.Photo_Seal_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for PSOT [Other - PSOT]
        [HttpGet]
        [Route("/Other/checkSubStage/PSOT")]
        public IActionResult checkingSubStageForPSOT()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.Photo_Seal_On_Trailer;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_Seal_On_Trailer;

            ViewBag.SubStageName = OtherProcess.Photo_Seal_On_Trailer;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherPhotoSealOnTrailer");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherPhotoSealOnTrailer");
        }


        // Other - PSOT - select a photo
        [HttpPost]
        [Route("/Other/PSOT/select-photo")]
        public IActionResult OtherPSOTSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Photo_Seal_On_Trailer;

            ViewBag.ProcessName = OtherProcess.Photo_Seal_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - PSOT - take a photo
        [HttpPost]
        [Route("/Other/PSOT/take-photo")]
        public IActionResult OtherPSOTTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_Seal_On_Trailer;
            ViewBag.ProcessName = OtherProcess.Photo_Seal_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for RLPL [Other - RLPL]
        [HttpGet]
        [Route("/Other/checkSubStage/RLPL")]
        public IActionResult checkingSubStageForRLPL()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.Reverse_Logistics_PackingList;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Reverse_Logistics_PackingList;

            ViewBag.SubStageName = OtherProcess.Reverse_Logistics_PackingList;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherReverseLogisticsPackingList");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherReverseLogisticsPackingList");
        }


        // Other - RLPL - select a photo
        [HttpPost]
        [Route("/Other/RLPL/select-photo")]
        public IActionResult OtherRLPLSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Reverse_Logistics_PackingList;

            ViewBag.ProcessName = OtherProcess.Reverse_Logistics_PackingList;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - RLPL - take a photo
        [HttpPost]
        [Route("/Other/RLPL/take-photo")]
        public IActionResult OtherRLPLTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Reverse_Logistics_PackingList;
            ViewBag.ProcessName = OtherProcess.Reverse_Logistics_PackingList;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for PNSOL [Other - PNSOL]
        [HttpGet]
        [Route("/Other/checkSubStage/PNSOL")]
        public IActionResult checkingSubStageForPNSOL()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.Photo_New_Seals_On_Lock;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_New_Seals_On_Lock;

            ViewBag.SubStageName = OtherProcess.Photo_New_Seals_On_Lock;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherPhotoNewSealsOnLock");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherPhotoNewSealsOnLock");
        }


        // Other - PNSOL - select a photo
        [HttpPost]
        [Route("/Other/PNSOL/select-photo")]
        public IActionResult OtherPNSOLSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Photo_New_Seals_On_Lock;

            ViewBag.ProcessName = OtherProcess.Photo_New_Seals_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - PNSOL - take a photo
        [HttpPost]
        [Route("/Other/PNSOL/take-photo")]
        public IActionResult OtherPNSOLTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_New_Seals_On_Lock;
            ViewBag.ProcessName = OtherProcess.Photo_New_Seals_On_Lock;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for PNSOT [Other - PNSOT]
        [HttpGet]
        [Route("/Other/checkSubStage/PNSOT")]
        public IActionResult checkingSubStageForPNSOT()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.Photo_New_Seals_On_Trailer;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_New_Seals_On_Trailer;

            ViewBag.SubStageName = OtherProcess.Photo_New_Seals_On_Trailer;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherPhotoNewSealsOnTrailer");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherPhotoNewSealsOnTrailer");
        }


        // Other - PNSOT select a photo
        [HttpPost]
        [Route("/Other/PNSOT/select-photo")]
        public IActionResult OtherPNSOTSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Photo_New_Seals_On_Trailer;

            ViewBag.ProcessName = OtherProcess.Photo_New_Seals_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - PNSOT - take a photo
        [HttpPost]
        [Route("/Other/PNSOT/take-photo")]
        public IActionResult OtherPNSOTTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_New_Seals_On_Trailer;
            ViewBag.ProcessName = OtherProcess.Photo_New_Seals_On_Trailer;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for PCS [Other - PCS]
        [HttpGet]
        [Route("/Other/checkSubStage/PCS")]
        public IActionResult checkingSubStageForPCS()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.Photo_Customs_Stamp;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_Customs_Stamp;

            ViewBag.SubStageName = OtherProcess.Photo_Customs_Stamp;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherPhotoCustomsStamp");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherPhotoCustomsStamp");
        }


        // Other - PCS select a photo
        [HttpPost]
        [Route("/Other/PCS/select-photo")]
        public IActionResult OtherPCSSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Photo_Customs_Stamp;

            ViewBag.ProcessName = OtherProcess.Photo_Customs_Stamp;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - PCS - take a photo
        [HttpPost]
        [Route("/Other/PCS/take-photo")]
        public IActionResult OtherPCSTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Photo_Customs_Stamp;
            ViewBag.ProcessName = OtherProcess.Photo_Customs_Stamp;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



        //checking sub stage for CMR-Customs [Other - CMR-Customs]
        [HttpGet]
        [Route("/Other/checkSubStage/CMR-Customs")]
        public IActionResult checkingSubStageForCMRCustoms()
        {
            TruckShipmentsDTOMultiPart truckShipmentsDTOMultiPart = new TruckShipmentsDTOMultiPart();
            TruckShipmentDTOBaseImg truckShipmentDTOBaseImg = new TruckShipmentDTOBaseImg();

            // checking for selected file is there
            truckShipmentsDTOMultiPart.ReferenceNo = ReferenceNo;
            truckShipmentsDTOMultiPart.Stage = StageNames.Other;
            truckShipmentsDTOMultiPart.ProcessName = OtherProcess.CMR_Customs;

            // checking for taken picture is there
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.CMR_Customs;

            ViewBag.SubStageName = OtherProcess.CMR_Customs;

            var data = truckAppServiceInterface.checkingSubStage(truckShipmentsDTOMultiPart, truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherCMRCustoms");
            }
            else if (data != null)
            {
                return View("OtherSubStageError");
            }
            return View("OtherCMRCustoms");
        }


        // Other - CMR-Customs select a photo
        [HttpPost]
        [Route("/Other/CMR-Customs/select-photo")]
        public IActionResult OtherCMRCustomsSelectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.CMR_Customs;

            ViewBag.ProcessName = OtherProcess.CMR_Customs;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        // Other - CMR-Customs - take a photo
        [HttpPost]
        [Route("/Other/CMR-Customs/take-photo")]
        public IActionResult OtherCMRCustomsTakeAPhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.CMR_Customs;
            ViewBag.ProcessName = OtherProcess.CMR_Customs;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        [HttpPost]
        [Route("/Other/checkStage/others")]
        public IActionResult Other_others_otherPic(string otherPic)
        {
            string special = "---";
            OthersDTO othersDTO = new OthersDTO();
            othersDTO.ReferenceNo = ReferenceNo;
            othersDTO.StageName = StageNames.Other;
            othersDTO.ProcessName = string.Concat(OtherProcess.Others.ToString(), special, otherPic);
            othersDTO.Image = null;

            ViewBag.ProcessName = otherPic;
            OthersPicName = otherPic;

            var data = truckAppServiceInterface.stageInsideOthersFileName(othersDTO);
            if (data == null)
            {
                return View("OthersInOther");
            }
            return View("OtherSubStageError");
        }



        [HttpPost]
        [Route("/Others/other/select-photo")]
        public IActionResult Others_Other_selectPhoto([FromForm] TruckShipmentsDTOMultiPart truckShipmentsDTO)
        {
            string special = "---";
            truckShipmentsDTO.ReferenceNo = ReferenceNo;
            truckShipmentsDTO.Stage = StageNames.Other;
            truckShipmentsDTO.ProcessName = OtherProcess.Others + special + OthersPicName;

            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToMultiPart(truckShipmentsDTO);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }


        [HttpPost]
        [Route("/Others/other/take-photo")]
        public IActionResult Others_Other_takePhoto(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg)
        {
            string special = "---";
            truckShipmentDTOBaseImg.ReferenceNo = ReferenceNo;
            truckShipmentDTOBaseImg.Stage = StageNames.Other;
            truckShipmentDTOBaseImg.ProcessName = OtherProcess.Others + special + OthersPicName; ;
            ViewBag.ProcessName = OthersPicName;

            var data = truckAppServiceInterface.cmrPostCollectionFromActionToBase64(truckShipmentDTOBaseImg);
            if (data == null)
            {
                return View("OtherSubStageSuccess");
            }
            return View("OtherSubStageSuccess");
        }



    }
}