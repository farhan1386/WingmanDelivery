import { type BaseModel } from "../shared/types";

export interface DeliveryLogsModel extends BaseModel {
    f_delivery_uid: string; // Guid mapping to parent delivery order context
    f_status_from: number;  // Status enum integer value before change
    f_status_to: number;    // Status enum integer value after change
}