using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecnologyShop.Models;

namespace TecnologyShop.Utility
{
    public class SD
    {

        //public const string DefaultFoodImage = "LogoLargoTS.png";
        public const string ManagerUser = "Manager"; //el mero bicho
        public const string SalesAgent = "SalesAgent"; //el agente que vendo en este caso seria como el kitchen del profe
        public const string CustomerUser = "Customer"; //el que compra
        public const string FrontDeskUser = "FrontDesk"; //el que modifica las vistas principales

        public const string ssShoppingCartCount = "ssCartCount";

        public const string ssCouponCode = "ssCouponCode";
        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for PickUp";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Canceled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        public static double DiscountedPrice(Coupon couponFromDb,
            double OriginalOrderTotal)
        {
            if (couponFromDb == null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (couponFromDb.MinimunAmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    //everything is valid
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Colones)
                    {
                        //$10 off $100
                        return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);
                    }
                    else
                    {
                        if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.percent)
                        {
                            //$10 off $100
                            return Math.Round(OriginalOrderTotal -
                                (OriginalOrderTotal * couponFromDb.Discount / 100), 2);
                        }
                    }
                }
                return OriginalOrderTotal;
            }
        }

        //imagen que se va a crear por default
        public const string DefaultProductImage = "DefaultImageProduct.png";




        //para poder renderizar imagenes en html
        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = true;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}
