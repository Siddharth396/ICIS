namespace BusinessLayer.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using BusinessLayer.DTO;
    using BusinessLayer.DTOs;

    public interface IContentBlockValidationService
    {
        public ValidationResponse ValidateContentBlockInput(SaveContentBlockRequest saveContentBlockRequest);
        public ValidationResponse ValidateContentBlockRequest(ContentBlockRequest contentBlockRequest);
    }
}
