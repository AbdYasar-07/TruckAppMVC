using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TruckAppMVC.DTO;
using TruckAppMVC.Model;

namespace TruckAppMVC.Service
{
    public interface ITruckAppServiceInterface
    {
        public FaceBookTracker validatingReferenceNo(string referenceNo);
        public TruckShipments cmrPostCollectionFromActionToMultiPart(TruckShipmentsDTOMultiPart truckShipmentsDTO);
        public StageOfShipment addingStageToDb(StageOfShipmentDTO stageOfShipmentDTO);
        public StageOfShipment StageTabChecking(string reference, string stage);
        public TruckShipments checkingSubStage(TruckShipmentsDTOMultiPart truckShipmentsDTO, TruckShipmentDTOBaseImg truckShipmentDTOBaseImg);
        public TruckShipments cmrPostCollectionFromActionToBase64(TruckShipmentDTOBaseImg truckShipmentDTOBaseImg);
        public TruckShipments stageInsideOthersFileName(OthersDTO othersDTO);
    }
}
