using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace TT_SOCTHDA_HDDT
{
    #region ReadonyConverter

    class BindingReadonly : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;

            string[] para = parameter.ToString().Split(new char[] { ';' });
            if (para.Length < 2)
                return true;

            switch (para[0])
            {
                case "PH":
                    switch (para[1])
                    {
                        case "ong_ba":
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                bool IsInEditMode = false;
                                bool.TryParse(values[0].ToString(), out IsInEditMode);
                                if (IsInEditMode == true && StartUp.M_ong_ba.Equals("1"))
                                {
                                    isreadonly = false;
                                }
                            }
                            break;
                        case "ngay_lct":
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                bool IsInEditMode = false;
                                bool.TryParse(values[0].ToString(), out IsInEditMode);
                                if (IsInEditMode == true && StartUp.M_ngay_lct.Equals("1"))
                                {
                                    if (values[0].ToString() == "1")
                                        isreadonly = false;
                                }
                            }
                            break;
                    }
                    break;
                case "CT":

                    if (true)
                    {
                        string paraStr = para[1];
                        switch (paraStr)
                        {
                            #region so_luong
                            case "so_luong":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue
                                           && values[2] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        //IsInEditMode = values[1].ToString().Equals("1") ? true : false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            bool IsCheckedPH_HDDT = false;
                                            bool.TryParse(values[2].ToString(), out IsCheckedPH_HDDT);
                                            if (IsCheckedPH_HDDT == true)
                                                isreadonly = false;
                                        }                                      

                                        if (values[0].ToString() == "")
                                            isreadonly = true;
                                    }
                                }
                                break;
                            #endregion

                            #region gia_nt2
                            case "gia_nt2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0;
                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            bool IsCheckedPH_HDDT = false;
                                            bool.TryParse(values[2].ToString(), out IsCheckedPH_HDDT);
                                            if (so_luong != 0 && IsCheckedPH_HDDT == true)
                                                isreadonly = false;
                                        }                                       
                                        
                                    }
                                }
                                break;
                            #endregion

                            #region tien_nt2
                            case "tien_nt2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt2 = 0;
                                            bool sua_tien = false;
                                            bool.TryParse(values[2].ToString(), out sua_tien);

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt2);

                                            if (so_luong == 0 || (so_luong * gia_nt2) == 0 || sua_tien == true)                                               
                                                isreadonly = false;
                                            
                                           
                                        }
                                         if (values[4].ToString() != "0") 
                                             isreadonly = true;
                                    }
                                }
                                break;
                            #endregion

                            #region gia_nt
                            case "gia_nt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0;
                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[1].ToString(), out gia_ton);
                                            bool Px_gia_dd = false;
                                            bool.TryParse(values[2].ToString(), out Px_gia_dd);
                                            if (so_luong != 0 && ((Px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && values[4].ToString() == "1")
                                                isreadonly = false;
                                        }
                                        bool IsCheckedPH_HDDT = false;
                                        bool.TryParse(values[5].ToString(), out IsCheckedPH_HDDT);
                                        if (IsCheckedPH_HDDT == false)
                                            isreadonly = true;
                                        
                                    }
                                }
                                break;
                            #endregion

                            #region tien_nt
                            case "tien_nt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue
                                        && values[6] != DependencyProperty.UnsetValue
                                        && values[7] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[4].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt = 0;
                                            bool sua_tien = false;
                                            bool.TryParse(values[3].ToString(), out sua_tien);
                                            bool Px_gia_dd = false;
                                            bool.TryParse(values[2].ToString(), out Px_gia_dd);

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt);

                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[6].ToString(), out gia_ton);

                                            bool IsCheckedPH_HDDT = false;
                                            bool.TryParse(values[7].ToString(), out IsCheckedPH_HDDT);

                                            if ((so_luong == 0 || (so_luong * gia_nt) == 0 || sua_tien == true)
                                                && ((Px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && values[5].ToString() == "1" && IsCheckedPH_HDDT == true)
                                                isreadonly = false;
                                        }
                                       
                                        
                                    }
                                }
                                break;
                            #endregion

                            #region gia2
                            case "gia2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt2 = 0, ty_gia = 0;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt2);
                                            bool.TryParse(values[2].ToString(), out sua_tien);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                            bool IsCheckedPH_HDDT = false;
                                            bool.TryParse(values[4].ToString(), out IsCheckedPH_HDDT);

                                            if (so_luong != 0 && ((gia_nt2 * ty_gia) == 0 || sua_tien == true) && IsCheckedPH_HDDT == true)
                                                //&& values[4].ToString() == "1")
                                                isreadonly = false;
                                        }                                      
                                        
                                    }

                                }
                                break;
                            #endregion

                            #region tien2
                            case "tien2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[2].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal tien_nt2 = 0, ty_gia = 0;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out tien_nt2);
                                            bool.TryParse(values[1].ToString(), out sua_tien);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                            if (sua_tien == true || (tien_nt2 * ty_gia) == 0)
                                                //|| values[3].ToString() == "0")
                                                isreadonly = false;
                                        }
                                        bool IsCheckedPH_HDDT = false;
                                        bool.TryParse(values[3].ToString(), out IsCheckedPH_HDDT);
                                        if (IsCheckedPH_HDDT == false)
                                            isreadonly = true;
                                    }

                                }
                                break;
                            #endregion
                            #region tl_ck
                            case "tl_ck":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue
                                           )
                                    {
                                        bool IsInEditMode = false;                                        
                                        bool.TryParse(values[0].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            bool IsCheckedPH_HDDT = false;
                                            bool.TryParse(values[1].ToString(), out IsCheckedPH_HDDT);
                                            if (IsCheckedPH_HDDT == true)
                                                isreadonly = false;
                                        }
                                        
                                    }
                                }
                                break;
                            #endregion
                            #region km_ck
                            case "km_ck":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue
                                           )
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[0].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            bool IsCheckedPH_HDDT = false;
                                            bool.TryParse(values[1].ToString(), out IsCheckedPH_HDDT);
                                            if (IsCheckedPH_HDDT == true)
                                                isreadonly = false;
                                        }

                                    }
                                }
                                break;
                            #endregion

                            #region ck_nt
                            case "ck_nt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal tl_ck = 0;
                                            decimal.TryParse(values[0].ToString(), out tl_ck);

                                            if (tl_ck != 0)
                                                isreadonly = false;
                                        }
                                        bool IsCheckedPH_HDDT = false;
                                        bool.TryParse(values[2].ToString(), out IsCheckedPH_HDDT);
                                        if (IsCheckedPH_HDDT == false)
                                            isreadonly = true;
                                    }

                                }
                                break;
                            #endregion

                            #region ck
                            case "ck":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[2].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal ck_nt = 0, ty_gia = 0;
                                            bool sua_tien = false;
                                            decimal tl_ck = 0;

                                            decimal.TryParse(values[0].ToString(), out ck_nt);
                                            bool.TryParse(values[1].ToString(), out sua_tien);
                                            decimal.TryParse(values[4].ToString(), out tl_ck);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                            if ((sua_tien == true || (ck_nt * ty_gia) == 0) && tl_ck != 0)
                                                isreadonly = false;
                                        }
                                        bool IsCheckedPH_HDDT = false;
                                        bool.TryParse(values[5].ToString(), out IsCheckedPH_HDDT);
                                        if (IsCheckedPH_HDDT == false)
                                            isreadonly = true;
                                    }


                                }
                                break;
                            #endregion

                            #region gia
                            case "gia":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue
                                        && values[6] != DependencyProperty.UnsetValue
                                        && values[7] != DependencyProperty.UnsetValue
                                        && values[8] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[4].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt = 0, ty_gia = 0;
                                            bool px_gia_dd = false;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt);
                                            bool.TryParse(values[2].ToString(), out sua_tien);
                                            bool.TryParse(values[3].ToString(), out px_gia_dd);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[7].ToString(), out gia_ton);

                                            if (so_luong != 0 && values[6].ToString() == "1"
                                                && ((px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && (sua_tien == true || gia_nt * ty_gia == 0))
                                                isreadonly = false;
                                        }
                                        bool IsCheckedPH_HDDT = false;
                                        bool.TryParse(values[8].ToString(), out IsCheckedPH_HDDT);
                                        if (IsCheckedPH_HDDT == false)
                                            isreadonly = true;
                                    }
                                }
                                break;
                            #endregion

                            #region tien
                            case "tien":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue
                                        && values[6] != DependencyProperty.UnsetValue
                                        && values[7] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal tien_nt = 0, ty_gia = 0;
                                            bool px_gia_dd = false;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out tien_nt);
                                            bool.TryParse(values[1].ToString(), out sua_tien);
                                            bool.TryParse(values[2].ToString(), out px_gia_dd);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[6].ToString(), out gia_ton);

                                            if ((sua_tien == true || tien_nt * ty_gia == 0)
                                                && ((px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && values[5].ToString() == "1")
                                                isreadonly = false;
                                        }
                                        bool IsCheckedPH_HDDT = false;
                                        bool.TryParse(values[7].ToString(), out IsCheckedPH_HDDT);
                                        if (IsCheckedPH_HDDT == false)
                                            isreadonly = true;
                                    }
                                }
                                break;
                            #endregion

                            #region tk thuế nợ, cục thuế trong ph
                            case "ma_kh2_ph":     
                            case "tk_thue_no_IsReadOnly":
                            case "tk_thue_no_AllowEmty":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            if (values[0].ToString() == "1" || (values.Length >2 && values[2].ToString() == "0"))
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region t_thue_nt_readOnly
                            case "t_thue_nt_readOnly":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    //&& values[2] != DependencyProperty.UnsetValue
                                    //&& values[3] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            //decimal t_tien_nt = 0, thue_suat = 0;
                                            //decimal.TryParse(values[0].ToString(), out t_tien_nt);
                                            //decimal.TryParse(values[1].ToString(), out thue_suat);

                                            //if (t_tien_nt * thue_suat == 0 || values[2].ToString() == "1")
                                            if (values[0].ToString() == "1")
                                                isreadonly = true;
                                            else
                                                isreadonly = false;
                                        }
                                        else
                                            isreadonly = false;
                                    }
                                }
                                break;
                            #endregion

                            #region t_thue_readOnly
                            case "t_thue_readOnly":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue
                                           && values[2] != DependencyProperty.UnsetValue)
                                    //&& values[3] != DependencyProperty.UnsetValue
                                    //&& values[4] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[2].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            //decimal t_tien = 0, thue_suat = 0;
                                            //decimal.TryParse(values[0].ToString(), out t_tien);
                                            //decimal.TryParse(values[1].ToString(), out thue_suat);

                                            // if (t_tien * thue_suat == 0 || (values[2].ToString() == "1" && values[3].ToString() == "1"))
                                            if (values[0].ToString() == "1" && values[1].ToString() == "1")
                                                isreadonly = true;
                                            else
                                                isreadonly = false;
                                        }
                                        else
                                            isreadonly = false;
                                    }
                                }
                                break;
                            #endregion

                            #region tk_vt
                            case "tk_vt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue 
                                        && values[3] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[2].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            //double sua_tk_vt = 0;
                                            //double.TryParse(values[0].ToString(), out sua_tk_vt);
                                            string tk_vt = values[1].ToString().Trim();
                                            string vt_ton_kho = values[3].ToString().Trim();
                                            //1223094
                                            if (vt_ton_kho == "1" && (values[0].ToString() == "1" || StartUp.IsTkMe(tk_vt)))
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tk_km_i
                            case "tk_km_i":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            //string tk_km_dmvt = values[2].ToString().Trim();
                                            if (values[0].ToString() == "1")// && (tk_km_dmvt == "" || StartUp.IsTkMe(tk_km_dmvt)))
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tk_ck
                            case "tk_ck":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue)
                                    //&& values[2] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {

                                            decimal tl_ck = 0;
                                            decimal.TryParse(values[0].ToString(), out tl_ck);
                                            // string tk_ck_dmvt = values[1].ToString().Trim();

                                            if (tl_ck != 0)// && (tk_ck_dmvt == "" || StartUp.IsTkMe(tk_ck_dmvt)))
                                                isreadonly = false;

                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tk_dt
                            case "tk_dt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            string tk_dt_dmvt = values[0].ToString().Trim();
                                            if (tk_dt_dmvt == "" || StartUp.IsTkMe(tk_dt_dmvt))
                                                isreadonly = false;

                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tk_gv
                            case "tk_gv":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            string tk_gv_dmvt = values[0].ToString().Trim();
                                            if (tk_gv_dmvt == "" || StartUp.IsTkMe(tk_gv_dmvt))
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tk_thue_co_IsReadOnly
                            case "tk_thue_co_IsReadOnly":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            string tk_thue_co_dmthue = values[0].ToString().Trim();
                                            if (tk_thue_co_dmthue == "" || StartUp.IsTkMe(tk_thue_co_dmthue))
                                                isreadonly = false;

                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region ma_post
                            case "ma_post":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            int sl_in = int.Parse(values[0].ToString());
                                            if (sl_in == 0)
                                                isreadonly = false;
                                        }
                                        else
                                            isreadonly = false;
                                    }
                                } break;
                            #endregion
                        }
                    }
                    break;
            }

            return isreadonly;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region IdRecordConverterXamDataGrid

    class IdRecordConverterXamDataGrid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    #endregion

    #region NotMaNt0Converter

    class NotMaNt0Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return (string)value == StartUp.SysObj.GetOption("M_MA_NT0").ToString() ?
            //    Visibility.Collapsed:Visibility.Visible;
            return StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString() == StartUp.SysObj.GetOption("M_MA_NT0").ToString() ?
              "Hidden" : "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region NotMaNt0Converter

    class StringEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value.ToString().Trim());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
