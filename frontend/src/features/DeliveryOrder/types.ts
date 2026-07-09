import type { BaseModel } from "../shared/types";

export const OrderStatus = {
    Pending: 0,
    InTransit: 1,
    Delivered: 2,
    Cancelled: 3,
} as const;

export type OrderStatusType = typeof OrderStatus[keyof typeof OrderStatus];

export interface DeliveryOrderModel extends BaseModel {
    f_pickup_address: string;
    f_status?: OrderStatusType | null;
    f_total_cost: number;
}