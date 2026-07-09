import { type BaseModel } from "../shared/types";

export interface RoleModel extends BaseModel {
    f_role_name: string;
    f_description: string;
    f_active: boolean;
}
