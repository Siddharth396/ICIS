// istanbul ignore file
import { Language } from 'types';

export type Messages = typeof messages.en;

const messages = {
  en: {
    General: {
      ExampleCardTitle: 'Price Entry',
      ExampleAuthoringCardTitle: 'Price Entry - Authoring',
      UnknownError: 'An unknown error occurred',
      Reload: 'Reload',
      TitlePlaceholder: 'Enter title',
      GridTitlePlaceholder: 'Enter grid title',
      ContentBlockTitlePlaceholder: 'Enter content block title',
      Save: 'Save',
      Cancel: 'Cancel',
      Delete: 'Delete',
      KeepEditing: 'Keep editing',
      GenericErrorMessage: 'Content failed to load',
      SelectPlaceholder: 'Please select',
      NoRecordsFound: 'No records found',
      SelectTooltipMessage:
        'You can only select options matching the criteria shown. To change this, clear your current selection.',
      DifferentScheduleTooltipMessage:
        'This series has a different schedule. Clear your selection or create a new table to add it',
      PriceSeriesAlreadySelectedTooltipMessage:
        'This price series is already selected in another table within this content block',
    },
    DeleteGridModal: {
      Title: 'Delete grid confirmation',
      AreYouSure: 'Are you sure you want to delete this grid?',
      CantBeUndone: 'This action cannot be undone.',
    },
    Capabilty: {
      SelectPriceSeries: 'Select prices',
      PriceCommentary: 'Price Commentary',
      WorkflowButtonTitle: 'Publish',
      ShowHideButtonTitle: 'Show/hide columns',
      PlaceholderTitle:
        'Select from Table configuration on the left to start populating the price series results',
    },
    Workflow: {
      GenericError: 'Table contains errors',
      NonPublishingDayWarning:
        'The selected date is a non-publishing day. Once the content has been reviewed and approved by Copy Editors, it will be published immediately.',
      StaticLabels: {
        NonMarketAdjustments: {
          Label: 'NMA',
          NmaApplied: 'Non-market adjustment applied',
          TooltipMessage: 'Prices and price changes are showing adjusted values',
        },
      },
      Actions: {
        START_NMA: 'Continue',
        CANCEL_NMA: 'Proceed',
        PUBLISH: 'Publish',
        CANCEL: 'Cancel correction',
        SEND_FOR_REVIEW: 'Send for review',
        APPROVE: 'Approve',
        SEND_BACK: 'Send back',
        START_REVIEW: 'Start review',
        PULL_BACK: 'Pull back',
        RELEASE_REVIEW: 'Release review',
        DEFAULT_ACTION: 'Submit',
        PUBLISH_CORRECTION: 'Publish',
        INITIATE_CORRECTION: 'Re-publish',
        SEND_FOR_REVIEW_CORRECTION: 'Send for review',
        APPROVE_CORRECTION: 'Approve',
        SEND_BACK_CORRECTION: 'Send back',
        START_REVIEW_CORRECTION: 'Start review',
      },
      EmptyPageValidationMessage: {
        Title: 'Prices not sent for review',
        Publish: {
          NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID:
            'Please complete all prices before they can be sent for review',
          NOT_ALL_INPUTS_ARE_PUBLISHED:
            'Please ensure all input price series are published before these can be sent for review',
          NOT_ALL_REFERENCE_PRICE_SERIES_ARE_PUBLISHED:
            'Please publish the referenced price series before publishing these assessments',
        },
        Correction: {
          CORRECTION_CONTAINS_NO_CHANGES_PARAGRAPH_1:
            'No changes have been made to the content since it was last published.',
          CORRECTION_CONTAINS_NO_CHANGES_PARAGRAPH_2:
            'Please make changes before it can be sent for review.',
        },
        DEFAULT: {
          NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID:
            'Please complete all prices before they can be sent for review',
        },
      },
      START_NMA: {
        SuccessMessage: {
          Title: 'Non-market adjustment',
          Submitted: 'Non-market adjustment started successfully!',
        },
        ConfirmationMessage: {
          Title: 'Non-market adjustment',
          ConfirmationInformation:
            'This will allow deltas to be editable and will not match price assessment changes.',
          ConfirmationQuestion: 'Would you like to proceed?',
        },
      },
      CANCEL_NMA: {
        SuccessMessage: {
          Title: 'Non-market adjustment',
          Submitted: 'Non-market adjustment cancelled successfully!',
        },
        ConfirmationMessage: {
          Title: 'Cancel Non-market adjustment',
          ConfirmationInformation: 'Manual changes made to deltas will be lost.',
          ConfirmationQuestion: 'Would you like to proceed?',
        },
      },
      PUBLISH: {
        SuccessMessage: {
          Title: 'Content Submitted',
          Submitted: 'Content submitted successfully for Publish',
        },
        ConfirmationMessage: {
          Title: 'Publish',
          ConfirmationInformation: 'You are about to publish the prices.',
          ConfirmationQuestion:
            'Have the prices been reviewed? Are you sure you would like to proceed?',
        },
      },
      PUBLISH_CORRECTION: {
        SuccessMessage: {
          Title: 'Content submitted',
          Submitted: 'Content submitted successfully for publish',
        },
        ConfirmationMessage: {
          Title: 'Publish',
          ConfirmationInformation: 'You are about to publish the prices.',
          ConfirmationQuestion:
            'Have the prices been reviewed? Are you sure you would like to proceed?',
        },
      },
      INITIATE_CORRECTION: {
        SuccessMessage: {
          Title: 'Edit prices',
          Submitted: 'Price is now ready to edit',
        },
        ConfirmationMessage: {
          Title: 'Correct prices',
          ConfirmationInformation:
            'You are about to initiate correction for the prices. This will move prices back to edit mode.',
          ConfirmationQuestion: 'Are you sure you would like to proceed with correction?',
        },
      },
      INITIATE_CORRECTION_CORRECTION: {
        ConfirmationMessage: {
          Title: '',
          ConfirmationInformation: '',
          ConfirmationQuestion: '',
        },
        SuccessMessage: {
          Title: 'Prices submitted',
          Submitted: 'Prices have been successfully sent for review',
        },
      },
      CANCEL: {
        SuccessMessage: {
          Title: 'Cancel correction',
          Submitted: 'Correction cancelled',
        },
        ConfirmationMessage: {
          Title: 'Cancel correction',
          ConfirmationInformation: 'Are you sure you want to cancel this correction?',
          ConfirmationQuestion: ' ',
        },
      },
      SEND_FOR_REVIEW: {
        SuccessMessage: {
          Title: 'Prices submitted',
          Submitted: 'Prices have been successfully sent for review',
        },
        ConfirmationMessage: {
          Title: 'Send for review',
          ConfirmationInformation: 'You are about to send the prices for review.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      SEND_FOR_REVIEW_CORRECTION: {
        ConfirmationMessage: {
          Title: '',
          ConfirmationInformation: '',
          ConfirmationQuestion: '',
        },
        SuccessMessage: {
          Title: 'Prices submitted',
          Submitted: 'Prices have been successfully sent for review',
        },
      },
      APPROVE: {
        SuccessMessage: {
          Title: 'Approve submission',
          Submitted: 'Submission approved',
        },
        ConfirmationMessage: {
          Title: 'Approve submission',
          ConfirmationInformation:
            'You are about to approve the submission which will publish prices to our subscribers across ICIS Clarity, ICIS API.',
          ConfirmationQuestion: 'Are you sure you would like to approve?',
        },
      },
      APPROVE_CORRECTION: {
        SuccessMessage: {
          Title: 'Approve correction request',
          Submitted: 'Correction request approved',
        },
        ConfirmationMessage: {
          Title: 'Approve correction request',
          ConfirmationInformation:
            'You\'re about to approve this correction which will be published to our subscribers across ICIS Clarity, ICIS API.',
          ConfirmationQuestion: ' ',
        },
      },
      SEND_BACK: {
        SuccessMessage: {
          Title: 'Send back to Editor',
          Submitted: 'The submission was sent back to the Editor',
        },
        ConfirmationMessage: {
          Title: 'Send back',
          ConfirmationInformation: 'You are about to send the prices back.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      SEND_BACK_CORRECTION: {
        SuccessMessage: {
          Title: 'Send back to author',
          Submitted: 'The correction request was returned to the author.',
        },
        ConfirmationMessage: {
          Title: 'Send back to author',
          ConfirmationInformation: 'You\'re about to return to the author for revision.',
          ConfirmationQuestion: 'Would you like to proceed?',
        },
      },
      START_REVIEW: {
        SuccessMessage: {
          Title: 'Review started',
          Submitted: 'Submission review started',
        },
        ConfirmationMessage: {
          Title: 'Start review',
          ConfirmationInformation: 'You are about to start reviewing the submission.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      START_REVIEW_CORRECTION: {
        SuccessMessage: {
          Title: 'Review started',
          Submitted: 'Submission review started',
        },
        ConfirmationMessage: {
          Title: 'Start review',
          ConfirmationInformation: 'You are about to start reviewing the submission.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      PULL_BACK: {
        SuccessMessage: {
          Title: 'Content pull back',
          Submitted: 'Content pulled back successfully',
        },
        ConfirmationMessage: {
          Title: 'Pull back',
          ConfirmationInformation: 'You are about to pull it back.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      PULL_BACK_CORRECTION: {
        SuccessMessage: {
          Title: 'Content pull back',
          Submitted: 'Content pulled back successfully',
        },
        ConfirmationMessage: {
          Title: 'Pull back',
          ConfirmationInformation: 'You are about to pull it back.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      RELEASE_REVIEW: {
        SuccessMessage: {
          Title: 'Submission released',
          Submitted: 'The submission released successfully',
        },
        ConfirmationMessage: {
          Title: 'Submission release',
          ConfirmationInformation: 'You are about to release reviewing.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      RELEASE_REVIEW_CORRECTION: {
        SuccessMessage: {
          Title: 'Submission released',
          Submitted: 'The submission released successfully',
        },
        ConfirmationMessage: {
          Title: 'Submission release',
          ConfirmationInformation: 'You are about to release reviewing.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      DEFAULT_MESSAGE: {
        SuccessMessage: {
          Title: 'Submitted',
          Submitted: 'Content submitted successfully!',
        },
        ConfirmationMessage: {
          Title: 'Submit',
          ConfirmationInformation: 'You are about to submit.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
      DEFAULT_ACTION: {
        SuccessMessage: {
          Title: 'Submitted',
          Submitted: 'Content submitted successfully!',
        },
        ConfirmationMessage: {
          Title: 'Submit',
          ConfirmationInformation: 'You are about to submit.',
          ConfirmationQuestion: 'Are you sure you would like to proceed?',
        },
      },
    },
    Errors: {
      MULTIPLE_SCHEDULES:
        'All Series should be of same schedule within one content block. Please add series of same schedule.',
      DUPLICATE_PRICE_SERIES_ID:
        'Selected price series has been already added to the content block. Please add other series.',
      MULTIPLE_COMMODITIES_IN_CONTENT_BLOCK:
        'Series from different commodities cannot be added within the same content block.',
      MULTIPLE_SERIES_ITEM_TYPE_CODES:
        'Series with different grid configuration cannot be added within the same grid',
    },
  },
  zh: {
    General: {
      ExampleCardTitle: '价格输入',
      ExampleAuthoringCardTitle: '价格输入 - 作者',
      UnknownError: '发生未知错误',
      Reload: '重新加载',
      TitlePlaceholder: '输入标题',
      GridTitlePlaceholder: '输入网格标题',
      ContentBlockTitlePlaceholder: '输入内容块标题',
      Save: '保存',
      Cancel: '取消',
      KeepEditing: '继续编辑',
      GenericErrorMessage: '内容加载失败',
      SelectPlaceholder: '请选择',
      NoRecordsFound: '未找到记录',
      SelectTooltipMessage: '您只能选择与所示条件匹配的选项。要更改此内容，请清除当前选择。',
      DifferentScheduleTooltipMessage: '此系列具有不同的计划。清除选择或创建新表以添加它',
      PriceSeriesAlreadySelectedTooltipMessage: '此价格系列已在此内容块中的另一个表中选定'
    },
    Capabilty: {
      SelectPriceSeries: '选择价格',
      PriceCommentary: '价格评论',
      WorkflowButtonTitle: '发布',
      ShowHideButtonTitle: '显示/隐藏列',
      PlaceholderTitle: '从左侧的表格配置中选择以开始填充价格系列结果',
    },
    Workflow: {
      GenericError: '表格中包含错误',
      StaticLabels: {
        NonMarketAdjustments: {
          Label: '国家气象局',
          NmaApplied: '采用非市场调整',
          TooltipMessage: '价格和价格变化显示调整后的值',
        },
      },
      Actions: {
        PUBLISH: '发布',
        CANCEL: '取消校正',
        SEND_FOR_REVIEW: '发送审核',
        APPROVE: '批准',
        SEND_BACK: '退回',
        START_REVIEW: '开始审核',
        PULL_BACK: '撤回',
        RELEASE_REVIEW: '释放审核',
        DEFAULT_ACTION: '提交',
        PUBLISH_CORRECTION: '发布',
        INITIATE_CORRECTION: '重新发布',
        SEND_FOR_REVIEW_CORRECTION: '发送审核',
        APPROVE_CORRECTION: '批准',
        SEND_BACK_CORRECTION: '退回',
        START_REVIEW_CORRECTION: '开始审核',
      },
      EmptyPageValidationMessage: {
        Title: '价格未提交审核',
        Publish: {
          NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID: '请先完成所有价格，然后才能提交审核',
          NOT_ALL_INPUTS_ARE_PUBLISHED: '请确保所有输入价格系列都已发布，然后才能提交审核',
          NOT_ALL_REFERENCE_PRICE_SERIES_ARE_PUBLISHED:
            '请先发布引用的价格系列，然后才能发布这些评估',
        },
        DEFAULT: {
          NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID: '请先完成所有价格，然后才能提交审核',
        },
      },
      START_NMA: {
        SuccessMessage: {
          Title: 'NMA已开始',
          Submitted: 'NMA已成功开始',
        },
        ConfirmationMessage: {
          Title: '非市场调整',
          ConfirmationInformation: '这将允许增量可编辑，不会匹配价格评估更改',
          ConfirmationQuestion: '您要继续吗？',
        },
      },
      CANCEL_NMA: {
        SuccessMessage: {
          Title: 'NMA已取消',
          Submitted: 'NMA已成功取消',
        },
        ConfirmationMessage: {
          Title: '取消非市场调整',
          ConfirmationInformation: 'COPY NEEDED: 将增量手动更改将丢失。',
          ConfirmationQuestion: '您要继续吗？',
        },
      },
      PUBLISH: {
        SuccessMessage: {
          Title: '已提交内容',
          Submitted: '已成功提交内容以供发布',
        },
        ConfirmationMessage: {
          Title: '发布',
          ConfirmationInformation: '您即将发布价格。',
          ConfirmationQuestion: '价格已审核？您确定要继续吗？',
        },
      },
      PUBLISH_CORRECTION: {
        SuccessMessage: {
          Title: '已提交内容',
          Submitted: '已成功提交内容以供发布',
        },
        ConfirmationMessage: {
          Title: '发布',
          ConfirmationInformation: '您即将发布价格。',
          ConfirmationQuestion: '价格已审核？您确定要继续吗？',
        },
      },
      INITIATE_CORRECTION: {
        SuccessMessage: {
          Title: '编辑价格',
          Submitted: '价格现在已准备好编辑',
        },
        ConfirmationMessage: {
          Title: '更正价格',
          ConfirmationInformation: '您即将开始更正价格。这将使价格返回到编辑模式。',
          ConfirmationQuestion: '您确定要继续进行更正吗？',
        },
      },
      INITIATE_CORRECTION_CORRECTION: {
        ConfirmationMessage: {
          Title: '',
          ConfirmationInformation: '',
          ConfirmationQuestion: '',
        },
        SuccessMessage: {
          Title: '已提交价格',
          Submitted: '已成功发送价格以供审核',
        },
      },
      CANCEL: {
        SuccessMessage: {
          Title: '取消更正',
          Submitted: '更正已取消',
        },
        ConfirmationMessage: {
          Title: '取消更正',
          ConfirmationInformation: '您确定要取消此更正吗？',
          ConfirmationQuestion: ' ',
        },
      },
      SEND_FOR_REVIEW: {
        SuccessMessage: {
          Title: '已提交价格',
          Submitted: '已成功发送价格以供审核',
        },
        ConfirmationMessage: {
          Title: '发送审核',
          ConfirmationInformation: '您即将发送价格以供审核。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      SEND_FOR_REVIEW_CORRECTION: {
        ConfirmationMessage: {
          Title: '',
          ConfirmationInformation: '',
          ConfirmationQuestion: '',
        },
        SuccessMessage: {
          Title: '已提交价格',
          Submitted: '已成功发送价格以供审核',
        },
      },
      APPROVE: {
        SuccessMessage: {
          Title: '批准提交',
          Submitted: '提交已批准',
        },
        ConfirmationMessage: {
          Title: '批准提交',
          ConfirmationInformation:
            '您即将批准此提交, 这将发布价格到我们的订阅者ICIS Clarity, ICIS API。',
          ConfirmationQuestion: '您确定要批准吗？',
        },
      },
      APPROVE_CORRECTION: {
        SuccessMessage: {
          Title: '批准更正请求',
          Submitted: '更正请求已批准',
        },
        ConfirmationMessage: {
          Title: '批准更正请求',
          ConfirmationInformation:
            '您即将批准此更正, 这将发布到我们的订阅者ICIS Clarity, ICIS API。',
          ConfirmationQuestion: ' ',
        },
      },
      SEND_BACK: {
        SuccessMessage: {
          Title: '发送回作者',
          Submitted: '提交已发送回作者',
        },
        ConfirmationMessage: {
          Title: '发送回',
          ConfirmationInformation: '您即将发送价格回。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      SEND_BACK_CORRECTION: {
        SuccessMessage: {
          Title: '发送回作者',
          Submitted: '更正请求已发送回作者',
        },
        ConfirmationMessage: {
          Title: '发送回作者',
          ConfirmationInformation: '您即将发送价格回。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      START_REVIEW: {
        SuccessMessage: {
          Title: '开始审核',
          Submitted: '提交审核已开始',
        },
        ConfirmationMessage: {
          Title: '开始审核',
          ConfirmationInformation: '您即将开始审核提交。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      START_REVIEW_CORRECTION: {
        SuccessMessage: {
          Title: '开始审核',
          Submitted: '提交审核已开始',
        },
        ConfirmationMessage: {
          Title: '开始审核',
          ConfirmationInformation: '您即将开始审核提交。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      PULL_BACK: {
        SuccessMessage: {
          Title: '撤回内容',
          Submitted: '内容已成功撤回',
        },
        ConfirmationMessage: {
          Title: '撤回',
          ConfirmationInformation: '您即将撤回。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      PULL_BACK_CORRECTION: {
        SuccessMessage: {
          Title: '撤回内容',
          Submitted: '内容已成功撤回',
        },
        ConfirmationMessage: {
          Title: '撤回',
          ConfirmationInformation: '您即将撤回。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      RELEASE_REVIEW: {
        SuccessMessage: {
          Title: '提交发布',
          Submitted: '提交发布成功',
        },
        ConfirmationMessage: {
          Title: '提交发布',
          ConfirmationInformation: '您即将发布审核。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      RELEASE_REVIEW_CORRECTION: {
        SuccessMessage: {
          Title: '提交发布',
          Submitted: '提交发布成功',
        },
        ConfirmationMessage: {
          Title: '提交发布',
          ConfirmationInformation: '您即将发布审核。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      DEFAULT_MESSAGE: {
        SuccessMessage: {
          Title: '已提交',
          Submitted: '内容已成功提交！',
        },
        ConfirmationMessage: {
          Title: '提交',
          ConfirmationInformation: '您即将提交。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
      DEFAULT_ACTION: {
        SuccessMessage: {
          Title: '已提交',
          Submitted: '内容已成功提交！',
        },
        ConfirmationMessage: {
          Title: '提交',
          ConfirmationInformation: '您即将提交。',
          ConfirmationQuestion: '您确定要继续吗？',
        },
      },
    },
    DeleteGridModal: {
      Title: '删除网格确认',
      AreYouSure: '您确定要删除此网格吗？',
      CantBeUndone: '此操作无 法撤消。',
    },
    Errors: {
      MULTIPLE_SCHEDULES: '所选日期有多个计划',
      DUPLICATE_PRICE_SERIES_ID: '找到重复的价格系列',
      MULTIPLE_COMMODITIES_IN_CONTENT_BLOCK: '同一内容块不允许多个商品',
      MULTIPLE_SERIES_ITEM_TYPE_CODES: '同一内容块不允许多个系列项类型代码',
    },
  },
};

/**
 * Gets the apps strings in the specified language.
 * @param locale The locale to get the messages in.
 */
// @ts-ignore
const translation = (locale: Language): Messages => messages[locale];

export default translation;
