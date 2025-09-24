// istanbul ignore file
// import { count, table } from 'console';
import { Language } from 'types';

export type Messages = typeof messages.en;

const messages = {
  en: {
    app: {
      title: 'Supply & Demand',
    },
    messages: {
      Error: 'An error has occurred. Please try again or contact admin.',
      Snapshot: 'This table shows latest data from {0}. Historical data for earlier periods is available upon request. Please contact ICIS customer support for further assistance.'
    },
    config: {
      title: 'Supply and Demand tables',
      addButton: 'Add table',
      tableHeader: 'Table Configuration',
      commodity: 'Commodity',
      region: 'Region',
      type: 'Type',
      save: 'Save',
      cancel: 'Cancel',
    },
    capacities: {
      title: 'Capacity changes',
      noRecords: {
        title: 'No developments or contractions have been reported',
        subtitle: 'Development and contractions will appear here once reported',
      },
      table: {
        location: 'Location',
        company: 'Company',
        site: 'Site',
        plantNo: 'Plant No.',
        type: 'Type',
        estimatedStart: 'Estimated start',
        newAnnualCapacity: 'New annual capacity (kt)',
        capacityChange: 'Capacity change (kt)',
        percentChange: '% Change',
        lastUpdated: 'Last updated'
      },
    },
    outages: {
      title: 'Shutdowns',
      noRecords: {
        title: 'No outages have been reported',
        subtitle: 'Outages will appear here once reported',
      },
      table: {
        outageStart: 'Start',
        outageEnd: 'End',
        location: 'Location',
        company: 'Company',
        site: 'Site',
        plantNo: 'Plant No.',
        cause: 'Cause',
        capacityLoss: 'Capacity loss',
        totalAnnualCapacity: 'Total annual capacity (kt)',
        lastUpdated: 'Last updated',
        comments: 'Comments',
      },
    },
  },
  zh: {
    app: {
      title: '供需',
    },
    messages: {
      Error: '发生了错误。请重试或联系管理员。',
      Snapshot: '该表显示了来自的最新数据 {0}. 早期历史数据可根据要求提供。请联系 ICIS 客户支持以获得进一步帮助。'
    },
    config: {
      title: '供需表',
      addButton: '添加表',
      tableHeader: '表配置',
      commodity: '商品',
      region: '地区',
      type: '类型',
      save: '保存',
      cancel: '取消',
    },
    capacities: {
      title: '容量变化',
      noRecords: {
        title: '没有报告任何发展或收缩',
        subtitle: '一旦报告，发展和收缩将显示在这里',
      },
      table: {
        location: '国家',
        company: '公司',
        site: '地点',
        plantNo: '工厂编号',
        type: '类型',
        estimatedStart: '预计开始',
        newAnnualCapacity: '新年产能 (kt)',
        capacityChange: '产能变化 (kt)',
        percentChange: '% 改变',
        lastUpdated: '最后更新'
      },
    },
    outages: {
      title: '关闭',
      noRecords: {
        title: '没有报告任何停机',
        subtitle: '一旦报告，停机将显示在这里',
      },
      table: {
        outageStart: '开始',
        outageEnd: '结束',
        location: '国家',
        company: '公司',
        site: '地点',
        plantNo: '工厂编号',
        cause: '原因',
        capacityLoss: '产能损失',
        totalAnnualCapacity: '总年产能 (kt)',
        lastUpdated: '最后更新',
        comments: '评论',
      },
    },
  },
};

/**
 * Gets the apps strings in the specified language.
 * @param locale The locale to get the messages in.
 */
const translation = (locale: Language): Messages => messages[locale];

export default translation;
