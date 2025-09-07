using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("stock", Schema = "tms")]
    public class Stock
    {
        [Key]
        [Column("stock_no")]
        public int StockNo { get; set; }

        [Column("division_no")]
        public short DivisionNo { get; set; }

        [Column("dept_no")]
        public short DeptNo { get; set; }

        [Column("opguide_no")]
        public short? OpGuideNo { get; set; }

        [Column("stock_analysisflag")]
        [StringLength(1)]
        public string StockAnalysisFlag { get; set; } = string.Empty;

        [Column("stock_checkflag")]
        [StringLength(1)]
        public string StockCheckFlag { get; set; } = string.Empty;

        [Column("stock_code")]
        [StringLength(10)]
        public string StockCode { get; set; } = string.Empty;

        [Column("stock_countflag")]
        [StringLength(1)]
        public string StockCountFlag { get; set; } = string.Empty;

        [Column("stock_extraitemflag")]
        [StringLength(1)]
        public string StockExtraItemFlag { get; set; } = string.Empty;

        [Column("stock_maxqty")]
        public int? StockMaxQty { get; set; }

        [Column("stock_itemflag")]
        [StringLength(1)]
        public string StockItemFlag { get; set; } = string.Empty;

        [Column("stock_kitflag")]
        [StringLength(1)]
        public string StockKitFlag { get; set; } = string.Empty;

        [Column("stock_name")]
        [StringLength(35)]
        public string StockName { get; set; } = string.Empty;

        [Column("stock_questqty")]
        public int? StockQuestQty { get; set; }

        [Column("stock_shortname")]
        [StringLength(15)]
        public string StockShortName { get; set; } = string.Empty;

        [Column("stock_status")]
        [StringLength(1)]
        public string StockStatus { get; set; } = string.Empty;

        [Column("stock_textflag")]
        [StringLength(1)]
        public string StockTextFlag { get; set; } = string.Empty;

        [Column("stock_vvno")]
        public short StockVvNo { get; set; }

        [Column("stocktype_code_1")]
        [StringLength(1)]
        public string StockTypeCode1 { get; set; } = string.Empty;

        [Column("stocktype_code_2")]
        [StringLength(1)]
        public string StockTypeCode2 { get; set; } = string.Empty;

        [Column("stock_gasflag")]
        [StringLength(1)]
        public string StockGasFlag { get; set; } = string.Empty;

        [Column("stock_vatno")]
        public short? StockVatNo { get; set; }

        [Column("stock_groupflag")]
        [StringLength(1)]
        public string StockGroupFlag { get; set; } = string.Empty;

        [Column("stock_volume")]
        public float? StockVolume { get; set; }

        [Column("stock_weight")]
        public float? StockWeight { get; set; }

        [Column("stockclass_code")]
        [StringLength(1)]
        public string StockClassCode { get; set; } = string.Empty;

        [Column("stock_unloadtime")]
        public short? StockUnloadTime { get; set; }

        [Column("stock_merchantflag")]
        [StringLength(1)]
        public string StockMerchantFlag { get; set; } = string.Empty;

        [Column("stockctrl_level")]
        public short StockCtrlLevel { get; set; }

        [Column("stock_regflag")]
        [StringLength(1)]
        public string StockRegFlag { get; set; } = string.Empty;

        [Column("fuel_type")]
        [StringLength(2)]
        public string FuelType { get; set; } = string.Empty;

        [Column("stock_fuelcapacity")]
        public float StockFuelCapacity { get; set; }

        [Column("productgroup_no")]
        public short ProductGroupNo { get; set; }

        [Column("stocktrans_no")]
        public short StockTransNo { get; set; }

        [Column("stock_ecodeformat")]
        [StringLength(1)]
        public string StockEcodeFormat { get; set; } = string.Empty;

        [Column("stock_serviceperiod")]
        public short StockServicePeriod { get; set; }

        [Column("stock_hazardflag")]
        [StringLength(1)]
        public string StockHazardFlag { get; set; } = string.Empty;

        [Column("stock_maintflag")]
        [StringLength(1)]
        public string StockMaintFlag { get; set; } = string.Empty;

        [Column("writeofflimit_levelno")]
        public short WriteoffLimitLevelNo { get; set; }

        [Column("stock_havlevel")]
        public float StockHavLevel { get; set; }

        [Column("stock_noiselevel")]
        public short StockNoiseLevel { get; set; }

        [Column("stock_lifeexpectancy")]
        public int StockLifeExpectancy { get; set; }

        [Column("risktype_code")]
        [StringLength(1)]
        public string RiskTypeCode { get; set; } = string.Empty;

        [Column("stock_packqty")]
        public int StockPackQty { get; set; }

        [Column("stock_catno")]
        public short StockCatNo { get; set; }

        [Column("stock_meterflag")]
        [StringLength(1)]
        public string StockMeterFlag { get; set; } = string.Empty;

        [Column("stock_batteryprint")]
        [StringLength(1)]
        public string StockBatteryPrint { get; set; } = string.Empty;

        [Column("delpromise_no")]
        public short DelPromiseNo { get; set; }

        [Column("stock_minqty")]
        public int StockMinQty { get; set; }

        [Column("stock_length")]
        public float StockLength { get; set; }

        [Column("stock_width")]
        public float StockWidth { get; set; }

        [Column("stock_height")]
        public float StockHeight { get; set; }

        [Column("stock_volume2")]
        public float StockVolume2 { get; set; }

        [Column("stock_stackable")]
        public int StockStackable { get; set; }

        [Column("stock_smalltool")]
        [StringLength(1)]
        public string StockSmallTool { get; set; } = string.Empty;

        [Column("accesstype_no")]
        public short AccessTypeNo { get; set; }

        [Column("stock_towable")]
        [StringLength(1)]
        public string StockTowable { get; set; } = string.Empty;

        [Column("stock_hiab")]
        [StringLength(1)]
        public string StockHiab { get; set; } = string.Empty;

        [Column("stock_towedweight")]
        public float StockTowedWeight { get; set; }

        [Column("stock_towedvolume")]
        public float StockTowedVolume { get; set; }

        [Column("stock_visinsp")]
        [StringLength(1)]
        public string StockVisInsp { get; set; } = string.Empty;

        [Column("stock_huflag")]
        [StringLength(1)]
        public string StockHuFlag { get; set; } = string.Empty;

        [Column("stock_cdcmaint")]
        [StringLength(1)]
        public string StockCdcMaint { get; set; } = string.Empty;

        [Column("ndeccategory_no")]
        public short NdecCategoryNo { get; set; }

        [Column("stock_apronflag")]
        [StringLength(1)]
        public string StockApronFlag { get; set; } = string.Empty;

        [Column("stock_opflag")]
        [StringLength(1)]
        public string StockOpFlag { get; set; } = string.Empty;

        [Column("stock_assetchargeflag")]
        [StringLength(1)]
        public string StockAssetChargeFlag { get; set; } = string.Empty;

        [Column("company_no")]
        public short CompanyNo { get; set; }

        [Column("stock_residual", TypeName = "decimal(14,2)")]
        public decimal StockResidual { get; set; }

        [Column("stock_trnperitem")]
        [StringLength(1)]
        public string StockTrnPerItem { get; set; } = string.Empty;

        [Column("stock_inspcert")]
        [StringLength(1)]
        public string StockInspCert { get; set; } = string.Empty;

        [Column("stock_uktariffcode")]
        [StringLength(10)]
        public string StockUkTariffCode { get; set; } = string.Empty;

        [Column("merchant_no")]
        public int MerchantNo { get; set; }

        [Column("stock_longname")]
        [StringLength(100)]
        public string StockLongName { get; set; } = string.Empty;

        [Column("stock_cissupply")]
        [StringLength(1)]
        public string StockCisSupply { get; set; } = string.Empty;

        [Column("stock_eutariffcode")]
        [StringLength(10)]
        public string StockEuTariffCode { get; set; } = string.Empty;

        [Column("tariffcoo_code")]
        [StringLength(3)]
        public string TariffCooCode { get; set; } = string.Empty;

        [Column("stock_sata")]
        [StringLength(1)]
        public string StockSatA { get; set; } = string.Empty;

        [Column("stock_satb")]
        [StringLength(1)]
        public string StockSatB { get; set; } = string.Empty;

        [Column("stock_satc")]
        [StringLength(1)]
        public string StockSatC { get; set; } = string.Empty;

        [Column("stock_satd")]
        [StringLength(1)]
        public string StockSatD { get; set; } = string.Empty;

        [Column("stock_sate")]
        [StringLength(1)]
        public string StockSatE { get; set; } = string.Empty;

        [Column("stock_satf")]
        [StringLength(1)]
        public string StockSatF { get; set; } = string.Empty;

        [Column("stock_satg")]
        [StringLength(1)]
        public string StockSatG { get; set; } = string.Empty;

        [Column("stock_sath")]
        [StringLength(1)]
        public string StockSatH { get; set; } = string.Empty;

        // Computed property for display
        public string DisplayName => !string.IsNullOrEmpty(StockName) ? StockName : 
                                   !string.IsNullOrEmpty(StockShortName) ? StockShortName : 
                                   $"Stock #{StockNo}";
    }
}
