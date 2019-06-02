using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.DocumentRepository.Interfaces
{
    public interface IDocumentDbInitializer
    {
        DocumentClient GetClient(string endpointUrl, string authorizationKey, ConnectionPolicy connectionPolicy = null);
    }
}
