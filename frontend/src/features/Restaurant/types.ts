import { type BaseModel } from "../shared/types";

export interface RestaurantModel extends BaseModel {
    f_name: string;
    f_address: string;
    f_contact?: string | null;
}
