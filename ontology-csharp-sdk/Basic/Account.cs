﻿using OntologyCSharpSDK.Interface;
using OntologyCSharpSDK.Network;
using System.Collections.Generic;
using OntologyCSharpSDK.Common;

namespace OntologyCSharpSDK.Basic
{
    class Account : IAccount
    {
        public static string _node { get; set; }

        public Account(string node)
        {
            _node = node;
        }

        public string createPrivateKey()
        {

            var bytes = Crypto.GetSecureRandomByteArray(32);
            var privatekey = Crypto.ByteArrayToHexString(bytes);
            return privatekey;
        }

        public string getPublicKey(string privatekey)
        {
            var publickey = TransactionBuilder.getPublicKey(privatekey);
            return publickey;
        }

        public string createONTID(string privatekey)
        {
            var hash = TransactionBuilder.getHash(privatekey);

            var ontid = "did:ont:" + TransactionBuilder.u160ToAddress(hash);

            return ontid;
        }


        public NetworkResponse registerONTID(string ontid, string privatekey)
        {
            var tx = TransactionBuilder.buildRegisterOntidTx(ontid, privatekey);
            var serialized = tx.serialize();
            IList<object> param = new List<object>() { serialized };
            var result = NetworkHelper.sendNetworkRequest(Protocol.REST, "POST", Constants.REST_sendRawTransaction, param);
            return result;
        }

        public string createAddressFromPublickKey(string publicKey)
        {
            publicKey = "1202" + publicKey;
            string programHash = TransactionBuilder.getSingleSigUInt160(publicKey);
            string address = TransactionBuilder.u160ToAddress(programHash);

            return address;
        }

        public NetworkResponse transferFund(string name, string fromaddress, string toaddress, decimal value, string privatekey)
        {
            var fromhexaddress = TransactionBuilder.AddresstTou160(fromaddress);
            var tohexaddress = TransactionBuilder.AddresstTou160(toaddress);
            var tx = TransactionBuilder.makeTransferTransaction(name, fromhexaddress, tohexaddress, value.ToString(), privatekey);
            var serialized = tx.serialize();

            IList<object> param = new List<object>() { serialized };
            var result = NetworkHelper.sendNetworkRequest(Protocol.REST, "POST", Constants.REST_sendRawTransaction, param);
            return result;
        }

        public NetworkResponse registerClaim(string context, string metadata,
            string content, string type, string issuer, string privatekey)
        {
            var claim = new Claim(context, content, metadata);
            var signed = claim.sign(privatekey);
            var claimId = claim.Id;
            var path = global::OntologyCSharpSDK.Common.Crypto.StringToHexString(claimId);
            var type_hex = global::OntologyCSharpSDK.Common.Crypto.StringToHexString(type);

            var data = "{'Type':'" + type + "','Value':{'Context':'" + context + "','Issuer':'" + issuer + "'}}";

            var value = claim.GetValue(data);

            var tx = TransactionBuilder.buildAddAttributeTx(path, value, type_hex, issuer, privatekey);

            var serialized = tx.serialize();
            IList<object> param = new List<object>() { serialized };
            var result = NetworkHelper.sendNetworkRequest(Protocol.REST, "POST", Constants.REST_sendRawTransaction, param);
            return result;
        }

        public NetworkResponse addPublicKey(string ontid, string new_publickey, string publickey, string privatekey)
        {
            var tx = TransactionBuilder.buildAddPublicKeyTx(ontid, new_publickey, publickey, privatekey);
            var serialized = tx.serialize();
            IList<object> param = new List<object>() { serialized };
            var result = NetworkHelper.sendNetworkRequest(Protocol.REST, "POST", Constants.REST_sendRawTransaction, param);
            return result;
        }

    }
}
