interface IBaseProps {
  contentBlockId: string
  isInPreviewMode: boolean
  isComponentMounted?: (isLoadedSuccessfully: boolean) => void
}

export default IBaseProps;
