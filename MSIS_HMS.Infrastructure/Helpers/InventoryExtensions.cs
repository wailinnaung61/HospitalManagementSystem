using System;
using MSIS_HMS.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using static MSIS_HMS.Infrastructure.Helpers.EfCoreExtensions;
using MSIS_HMS.Infrastructure.Data;

namespace MSIS_HMS.Infrastructure.Helpers
{
    public static class InventoryExtensions
    {

        public static int GetSmallestQty(this Item item, int UnitId, int Qty)
        {
            var result = 0;
            var smallestUnitLevel = item.PackingUnits.SingleOrDefault(x => x.UnitId == UnitId)?.UnitLevel;
            if (smallestUnitLevel > 0)
            {
                var packingUnits = item.PackingUnits.OrderByDescending(x => x.UnitLevel).Where(x => x.UnitLevel <= smallestUnitLevel).ToList();
                for (var i = 0; i < packingUnits.Count(); i++)
                {
                    if (i == 0)
                    {
                        result = packingUnits[i].QtyInParent;
                    }
                    else
                    {
                        result *= packingUnits[i].QtyInParent;
                    }

                }
            }
            return result;
        }
        public static IDictionary<int, int> GetUnitFromSamllestQty(this List<PackingUnit> packingUnit, int Qty)
        {
            decimal result = 0;
            IDictionary<int, int> numberNames = new Dictionary<int, int>();
            var packingUnits = packingUnit.OrderByDescending(x => x.UnitLevel).ToList();
            if (Qty <= 0)
            {
                numberNames.Add(packingUnits.Last().UnitId, 0);
                return numberNames;
            }
            for (var i = 0; i < packingUnits.Count(); i++)
            {

                //if (numberNames.Last().Key != packingUnits.Last().UnitId)
                //{
                //}

                if (i == 0)
                {
                    if (Qty <= packingUnits[i].QtyInParent)
                    {
                        numberNames.Add(packingUnits[i].UnitId, Qty);
                        return numberNames;
                    }

                    result = (decimal)Qty / packingUnits[i].QtyInParent;
                    List<int> res = SplitDecimalPoint(result, packingUnits[i].QtyInParent);
                    if (res[1] > packingUnits[i].QtyInParent)
                    {
                        var smallestUnitQty = (decimal)res[1] / packingUnit[i].QtyInParent;
                        List<int> res1 = SplitDecimalPoint(smallestUnitQty, packingUnits[i].QtyInParent);
                        if (res1.Count == 1)
                        {
                            numberNames.Add(packingUnits[i + 1].UnitId, res1[0]);
                        }
                    }
                    else
                    {
                           
                        
                        if (packingUnits.Count > 1)
                        {
                            numberNames.Add(packingUnits[i].UnitId, res[1]);
                            if (res[0] <= packingUnits[i + 1].QtyInParent)
                            {
                                numberNames.Add(packingUnits[i + 1].UnitId, res[0]);
                            }
                            else
                            {
                                result = res[0];//(decimal)res[0] / packingUnits[i + 1].QtyInParent;
                                                //List<int> res1 = SplitDecimalPoint(result, packingUnits[i + 1].QtyInParent);
                                                //if (res1.Count > 1)
                                                //{
                                                //    numberNames.Add(packingUnits[i].UnitId, res1[1]);
                                                //    numberNames.Add(packingUnits[i + 1].UnitId, res1[0]);
                                                //}
                                                //else
                                                //{

                                //    numberNames.Add(packingUnits[i + 1].UnitId, res1[0]);
                                //}



                            }
                        }
                        else
                        {
                            numberNames.Add(packingUnits[i].UnitId, res[0]);
                        }

                    }



                }
                else
                {
                    if (i != packingUnits.Count - 1)
                    {
                        if (numberNames.Last().Key != packingUnits.Last().UnitId && result > packingUnits[i].QtyInParent)
                        {

                            result = (decimal)result / packingUnits[i].QtyInParent;

                            List<int> res = SplitDecimalPoint(result, packingUnits[i].QtyInParent);
                            if (res[1] > packingUnits[i].QtyInParent && packingUnits[i].UnitLevel != 1)
                            {
                                var smallestUnitQty = (decimal)res[1] / packingUnit[i].QtyInParent;
                                List<int> res1 = SplitDecimalPoint(smallestUnitQty, packingUnits[i].QtyInParent);
                                if (res1.Count == 1)
                                {
                                    numberNames.Add(packingUnits[i + 1].UnitId, res1[0]);
                                }
                                else
                                {
                                    numberNames.Add(packingUnits[i].UnitId, res1[1]);
                                    result = res[0] + res1[0];
                                    numberNames.Add(packingUnits[i + 1].UnitId, Convert.ToInt32(result));
                                }
                            }
                            else
                            {
                                numberNames.Add(packingUnits[i].UnitId, res[1]);
                                numberNames.Add(packingUnits[i + 1].UnitId, res[0]);
                                //if (packingUnits[i].UnitLevel != 1)
                                //{
                                //    if (res[0] <= packingUnits[i + 1].QtyInParent)
                                //    {
                                //        numberNames.Add(packingUnits[i + 1].UnitId, res[0]);
                                //    }

                                //}
                            }

                        }

                    }


                }

            }

            return numberNames;
        }
        private static List<int> SplitDecimalPoint(decimal result, int QtyInParent)
        {
            result = decimal.Round(result, 2, MidpointRounding.AwayFromZero);
            List<int> lst = new List<int>();
            if (result.ToString().Contains('.'))
            {
                var splitVal = result.ToString().Split('.',2);
                if (Convert.ToInt64(splitVal[1]) > QtyInParent)
                {
                    var decimalVal = (decimal)Convert.ToInt64(splitVal[1]) / QtyInParent;
                    var splitDec = decimalVal.ToString("0.00").Split('.');
                    var sumVal = Convert.ToInt32(splitVal[0]) + Convert.ToInt32(splitDec[0]);
                    lst.Add(sumVal);
                    lst.Add(Convert.ToInt32(splitDec[1]));
                }
                else
                {
                    lst.Add(Convert.ToInt32(splitVal[0]));
                    lst.Add(Convert.ToInt32(splitVal[1]));
                }
            }
            else
            {
                lst.Add(Convert.ToInt32(result));
                lst.Add(0);
            }
            return lst;
        }
        public static decimal CalculateTotal<T>(this ICollection<T> sender)
        {
            decimal total = 0;
            if (sender != null)
            {
                foreach (var item in sender)
                {
                    int qty = item.GetValue<T, int>("Qty");
                    decimal unitPrice = item.GetValue<T, decimal>("UnitPrice");
                    total += qty * unitPrice;
                }
            }
            return total;
        }
        public static decimal CalculateFeeTotal<T>(this ICollection<T> sender)
        {
            decimal total = 0;
            if (sender != null)
            {
                foreach (var item in sender)
                {
                    //int qty = item.GetValue<T, int>("Fee");
                    decimal fee = item.GetValue<T, decimal>("Fee");
                    total += fee;
                }
            }
            return total;
        }
        public static decimal CalculateBulkTotal(DeliverOrder deliverOrder)
        {
            decimal total = 0;
            decimal bulktotal = 0;
            foreach (var item in deliverOrder.DeliverOrderItems)
            {
                int qty = item.GetValue<DeliverOrderItem, int>("Qty");
                decimal unitPrice = item.GetValue<DeliverOrderItem, decimal>("UnitPrice");
                total += qty * unitPrice;
                //bulktotal=(total-(total * (deliverOrder.Discount/100)))+deliverOrder.Tax;
                bulktotal = (total + deliverOrder.Tax) - deliverOrder.Discount;
            }

            return bulktotal;
        }

        public static List<WarehouseItem> GetWarehouseItemForUpdate<T>(this ICollection<T> entities, ICollection<WarehouseItem> warehouseItems, int warehouseId)
        {
            var _warehouseItemsForUpdate = new List<WarehouseItem>();
            var _newItemsForStock = entities.GroupBy(x => new { WarehouseId = warehouseId, ItemId = (int)x.GetValue("ItemId"), BatchId = (int)x.GetValue("BatchId") }, (key, g) => new WarehouseItem
            {
                WarehouseId = warehouseId,
                ItemId = (int)g.First().GetValue("ItemId"),
                BatchId = (int)g.First().GetValue("BatchId"),
                Qty = g.Sum(i => (int)i.GetValue("QtyInSmallestUnit"))
            }).ToList();
            foreach (var item in _newItemsForStock)
            {
                var _existingPurchaseItemInStock = warehouseItems.SingleOrDefault(x => (int)x.GetValue("WarehouseId") == warehouseId && (int)x.GetValue("ItemId") == item.ItemId && (int)x.GetValue("BatchId") == item.BatchId);
                int? qtyChanged = null;
                if (_existingPurchaseItemInStock != null)
                {
                    // Update
                    qtyChanged = item.Qty - _existingPurchaseItemInStock.Qty;
                }
                else
                {
                    // Add
                    qtyChanged = item.Qty;
                }
                if (qtyChanged != null)
                {
                    _warehouseItemsForUpdate.Add(new WarehouseItem
                    {
                        WarehouseId = warehouseId,
                        ItemId = item.ItemId,
                        BatchId = item.BatchId,
                        Qty = (int)qtyChanged
                    });
                }
            }
            return _warehouseItemsForUpdate;
        }

        public static List<WarehouseItem> GetWarehouseItemForDelete<T>(this ICollection<T> entities, ICollection<WarehouseItem> warehouseItems, int warehouseId)
        {
            entities.ToList().ForEach(x => x.SetValue("QtyInSmallestUnit", 0));
            return entities.GetWarehouseItemForUpdate(warehouseItems, warehouseId);
        }

        public static List<OutletItem> GetOutletItemForUpdate<T>(this ICollection<T> entities, ICollection<OutletItem> outletItems, int outletId)
        {
            var _outletItemsForUpdate = new List<OutletItem>();
            var _newItemsForStock = entities.GroupBy(x => new { OutletId = outletId, ItemId = (int)x.GetValue("ItemId") }, (key, g) => new OutletItem
            {
                OutletId = outletId,
                ItemId = (int)g.First().GetValue("ItemId"),
                Qty = g.Sum(i => (int)i.GetValue("QtyInSmallestUnit"))
            }).ToList();
            foreach (var item in _newItemsForStock)
            {
                var _existingPurchaseItemInStock = outletItems.SingleOrDefault(x => (int)x.GetValue("OutletId") == outletId && (int)x.GetValue("ItemId") == item.ItemId);
                int? qtyChanged = null;
                if (_existingPurchaseItemInStock != null)
                {
                    // Update
                    qtyChanged = item.Qty - _existingPurchaseItemInStock.Qty;
                }
                else
                {
                    // Add
                    qtyChanged = item.Qty;
                }
                if (qtyChanged != null)
                {
                    _outletItemsForUpdate.Add(new OutletItem
                    {
                        OutletId = outletId,
                        ItemId = item.ItemId,
                        Qty = (int)qtyChanged
                    });
                }
            }
            return _outletItemsForUpdate;
        }

        public static List<OutletItem> GetOutletItemForDelete<T>(this ICollection<T> entities, ICollection<OutletItem> outletItems, int outletId)
        {
            entities.ToList().ForEach(x => x.SetValue("QtyInSmallestUnit", 0));
            return entities.GetOutletItemForUpdate(outletItems, outletId);
        }
    }
}
