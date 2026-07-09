import { type BaseModel } from "../shared/types";

export interface OrderItemModel extends BaseModel {
    f_order_uid: string;     // Guid linking back to primary transaction order
    f_menu_item_uid: string; // Guid identifying specific item entry selection
    f_quantity: number;      // Quantity units count
}
