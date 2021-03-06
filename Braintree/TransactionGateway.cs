#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Xml;
using Braintree.Exceptions;

#if NET452
using System.Threading.Tasks;
#endif

namespace Braintree
{
    /// <summary>
    /// Provides operations for sales, credits, refunds, voids, submitting for settlement, and searching for transactions in the vault
    /// </summary>
    public class TransactionGateway : ITransactionGateway
    {
        private readonly BraintreeService service;
        private readonly IBraintreeGateway gateway;

        protected internal TransactionGateway(IBraintreeGateway gateway)
        {
            gateway.Configuration.AssertHasAccessTokenOrKeys();
            this.gateway = gateway;
            service = new BraintreeService(gateway.Configuration);
        }

        [Obsolete("Use gateway.TransparentRedirect.Url")]
        public virtual string TransparentRedirectURLForCreate()
        {
            return service.BaseMerchantURL() + "/transactions/all/create_via_transparent_redirect_request";
        }

        public virtual Result<Transaction> CancelRelease(string id)
        {
            var request = new TransactionRequest();

            XmlNode response = service.Put(service.MerchantPath() + "/transactions/" + id + "/cancel_release", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> CancelReleaseAsync(string id)
        {
            var request = new TransactionRequest();

            XmlNode response = await service.PutAsync(service.MerchantPath() + "/transactions/" + id + "/cancel_release", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        [Obsolete("Use gateway.TransparentRedirect.Confirm()")]
        public virtual Result<Transaction> ConfirmTransparentRedirect(string queryString)
        {
            var trRequest = new TransparentRedirectRequest(queryString, service);
            XmlNode node = service.Post(service.MerchantPath() + "/transactions/all/confirm_transparent_redirect_request", trRequest);

            return new ResultImpl<Transaction>(new NodeWrapper(node), gateway);
        }

        public virtual Result<Transaction> HoldInEscrow(string id)
        {
            var request = new TransactionRequest();

            XmlNode response = service.Put(service.MerchantPath() + "/transactions/" + id + "/hold_in_escrow", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> HoldInEscrowAsync(string id)
        {
            var request = new TransactionRequest();

            XmlNode response = await service.PutAsync(service.MerchantPath() + "/transactions/" + id + "/hold_in_escrow", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual string SaleTrData(TransactionRequest trData, string redirectURL)
        {
            trData.Type = TransactionType.SALE;

            return TrUtil.BuildTrData(trData, redirectURL, service);
        }

        public virtual string CreditTrData(TransactionRequest trData, string redirectURL)
        {
            trData.Type = TransactionType.CREDIT;

            return TrUtil.BuildTrData(trData, redirectURL, service);
        }

        public virtual Result<Transaction> Credit(TransactionRequest request)
        {
            request.Type = TransactionType.CREDIT;
            XmlNode response = service.Post(service.MerchantPath() + "/transactions", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> CreditAsync(TransactionRequest request)
        {
            request.Type = TransactionType.CREDIT;
            XmlNode response = await service.PostAsync(service.MerchantPath() + "/transactions", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Transaction Find(string id)
        {
            if(id == null || id.Trim().Equals(""))
                throw new NotFoundException();

            XmlNode response = service.Get(service.MerchantPath() + "/transactions/" + id);

            return new Transaction(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Transaction> FindAsync(string id)
        {
            if (id == null || id.Trim().Equals(""))
                throw new NotFoundException();

            XmlNode response = await service.GetAsync(service.MerchantPath() + "/transactions/" + id);

            return new Transaction(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> Refund(string id)
        {
            XmlNode response = service.Post(service.MerchantPath() + "/transactions/" + id + "/refund");
            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> RefundAsync(string id)
        {
            XmlNode response = await service.PostAsync(service.MerchantPath() + "/transactions/" + id + "/refund");
            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> Refund(string id, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount
            };
            XmlNode response = service.Post(service.MerchantPath() + "/transactions/" + id + "/refund", request);
            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> RefundAsync(string id, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount
            };
            XmlNode response = await service.PostAsync(service.MerchantPath() + "/transactions/" + id + "/refund", request);
            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> Sale(TransactionRequest request)
        {
            request.Type = TransactionType.SALE;
            XmlNode response = service.Post(service.MerchantPath() + "/transactions", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> SaleAsync(TransactionRequest request)
        {
            request.Type = TransactionType.SALE;
            XmlNode response = await service.PostAsync(service.MerchantPath() + "/transactions", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> ReleaseFromEscrow(string id)
        {
            var request = new TransactionRequest();

            XmlNode response = service.Put(service.MerchantPath() + "/transactions/" + id + "/release_from_escrow", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> ReleaseFromEscrowAsync(string id)
        {
            var request = new TransactionRequest();

            XmlNode response = await service.PutAsync(service.MerchantPath() + "/transactions/" + id + "/release_from_escrow", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> SubmitForSettlement(string id)
        {
            return SubmitForSettlement(id, 0);
        }

#if NET452
        public virtual async Task<Result<Transaction>> SubmitForSettlementAsync(string id)
        {
            return await SubmitForSettlementAsync(id, 0);
        }
#endif

        public virtual Result<Transaction> SubmitForSettlement(string id, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount
            };
            return SubmitForSettlement(id, request);
        }

#if NET452
        public virtual async Task<Result<Transaction>> SubmitForSettlementAsync(string id, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount
            };
            return await SubmitForSettlementAsync(id, request);
        }
#endif

        public virtual Result<Transaction> SubmitForSettlement(string id, TransactionRequest request)
        {
            XmlNode response = service.Put(service.MerchantPath() + "/transactions/" + id + "/submit_for_settlement", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> SubmitForSettlementAsync(string id, TransactionRequest request)
        {
            XmlNode response = await service.PutAsync(service.MerchantPath() + "/transactions/" + id + "/submit_for_settlement", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> UpdateDetails(string id, TransactionRequest request)
        {
            XmlNode response = service.Put(service.MerchantPath() + "/transactions/" + id + "/update_details", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> UpdateDetailsAsync(string id, TransactionRequest request)
        {
            XmlNode response = await service.PutAsync(service.MerchantPath() + "/transactions/" + id + "/update_details", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> SubmitForPartialSettlement(string id, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount
            };
            return SubmitForPartialSettlement(id, request);
        }

#if NET452
        public virtual async Task<Result<Transaction>> SubmitForPartialSettlementAsync(string id, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount
            };
            return await SubmitForPartialSettlementAsync(id, request);
        }
#endif

        public virtual Result<Transaction> SubmitForPartialSettlement(string id, TransactionRequest request)
        {
            XmlNode response = service.Post(service.MerchantPath() + "/transactions/" + id + "/submit_for_partial_settlement", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> SubmitForPartialSettlementAsync(string id, TransactionRequest request)
        {
            XmlNode response = await service.PostAsync(service.MerchantPath() + "/transactions/" + id + "/submit_for_partial_settlement", request);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual Result<Transaction> Void(string id)
        {
            XmlNode response = service.Put(service.MerchantPath() + "/transactions/" + id + "/void");

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> VoidAsync(string id)
        {
            XmlNode response = await service.PutAsync(service.MerchantPath() + "/transactions/" + id + "/void");

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        public virtual ResourceCollection<Transaction> Search(TransactionSearchRequest query)
        {
            var response = new NodeWrapper(service.Post(service.MerchantPath() + "/transactions/advanced_search_ids", query));

            if (response.GetName() == "search-results") {
                return new ResourceCollection<Transaction>(response, delegate(string[] ids) {
                    return FetchTransactions(query, ids);
                });
            } else {
                throw new DownForMaintenanceException();
            }
        }

#if NET452
        public virtual async Task<ResourceCollection<Transaction>> SearchAsync(TransactionSearchRequest query)
        {
            var response = new NodeWrapper(await service.PostAsync(service.MerchantPath() + "/transactions/advanced_search_ids", query));

            if (response.GetName() == "search-results")
            {
                // TODO async
                return new ResourceCollection<Transaction>(response, delegate (string[] ids) {
                    return FetchTransactions(query, ids);
                });
            }
            else
            {
                throw new DownForMaintenanceException();
            }
        }
#endif

        public virtual Result<Transaction> CloneTransaction(string id, TransactionCloneRequest cloneRequest)
        {
            XmlNode response = service.Post(service.MerchantPath() + "/transactions/" + id + "/clone", cloneRequest);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }

#if NET452
        public virtual async Task<Result<Transaction>> CloneTransactionAsync(string id, TransactionCloneRequest cloneRequest)
        {
            XmlNode response = await service.PostAsync(service.MerchantPath() + "/transactions/" + id + "/clone", cloneRequest);

            return new ResultImpl<Transaction>(new NodeWrapper(response), gateway);
        }
#endif

        private List<Transaction> FetchTransactions(TransactionSearchRequest query, string[] ids)
        {
            query.Ids.IncludedIn(ids);

            var response = new NodeWrapper(service.Post(service.MerchantPath() + "/transactions/advanced_search", query));

            var transactions = new List<Transaction>();
            foreach (var node in response.GetList("transaction"))
            {
                transactions.Add(new Transaction(node, gateway));
            }
            return transactions;
        }

#if NET452
        // TODO async delegate in ResourceCollection constructor
        private async Task<List<Transaction>> FetchTransactionsAsync(TransactionSearchRequest query, string[] ids)
        {
            query.Ids.IncludedIn(ids);

            var response = new NodeWrapper(await service.PostAsync(service.MerchantPath() + "/transactions/advanced_search", query));

            var transactions = new List<Transaction>();
            foreach (var node in response.GetList("transaction"))
            {
                transactions.Add(new Transaction(node, gateway));
            }
            return transactions;
        }
#endif
    }
}
