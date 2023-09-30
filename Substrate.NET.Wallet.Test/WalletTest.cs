using NUnit.Framework;
using Substrate.NET.Wallet;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System;
using System.IO;
using System.Text;

namespace SubstrateNetWalletTest
{
    public class WalletTest
    {
        [SetUp]
        public void Setup()
        {
            SystemInteraction.ReadData = f => File.ReadAllText(Path.Combine(Environment.CurrentDirectory, f));
            SystemInteraction.DataExists = f => File.Exists(Path.Combine(Environment.CurrentDirectory, f));
            SystemInteraction.ReadPersistent = f => File.ReadAllText(Path.Combine(Environment.CurrentDirectory, f));
            SystemInteraction.PersistentExists = f => File.Exists(Path.Combine(Environment.CurrentDirectory, f));
            SystemInteraction.Persist = (f, c) => File.WriteAllText(Path.Combine(Environment.CurrentDirectory, f), c);
        }

        [Test]
        public void IsValidPasswordTest()
        {
            Assert.False(Wallet.IsValidPassword("12345678"));
            Assert.False(Wallet.IsValidPassword("ABCDEFGH"));
            Assert.False(Wallet.IsValidPassword("abcdefgh"));
            Assert.False(Wallet.IsValidPassword("ABCDefgh"));
            Assert.False(Wallet.IsValidPassword("1BCDefg"));

            Assert.True(Wallet.IsValidPassword("ABCDefg1"));
        }

        [Test]
        public void IsValidWalletNameTest()
        {
            Assert.False(Wallet.IsValidWalletName("1234"));
            Assert.False(Wallet.IsValidWalletName("ABC_/"));

            Assert.True(Wallet.IsValidWalletName("wal_let"));
            Assert.True(Wallet.IsValidWalletName("1111111"));
        }

        [Test]
        public void LoadWalletFromFileTest()
        {
            var walletName1 = "dev_wallet1";
            Wallet.Load(walletName1, out Wallet wallet1);
            Assert.True(wallet1.IsStored);
            Assert.False(wallet1.IsUnlocked);
            Assert.AreEqual("Ed25519",
                wallet1.FileStore.KeyType.ToString());
            Assert.AreEqual("5FfzQe73TTQhmSQCgvYocrr6vh1jJXEKB8xUB6tExfpKVCEZ",
                Utils.GetAddressFrom(wallet1.FileStore.PublicKey));
            Assert.AreEqual("0x17E39AC65C894EC263396E9B8720D78A7A5FE0CB6C5C05DC32E756DF3D5D2D9622DBFDB41CE0C9067B810BB03E1DCE9C89CFC061FBB063B616FF91F3AA31498158632A35601C91DFEE5DA869D44FA8A4",
                Utils.Bytes2HexString(wallet1.FileStore.EncryptedSeed));
            Assert.AreEqual("0x34F0627DB7C9BF1B580A597122622E95",
                Utils.Bytes2HexString(wallet1.FileStore.Salt));
            wallet1.Unlock("aA1234dd");
            Assert.True(wallet1.IsUnlocked);

            var walletName2 = "dev_wallet2";
            Wallet.Load(walletName2, out Wallet wallet2);
            Assert.True(wallet2.IsStored);
            Assert.False(wallet2.IsUnlocked);
            Assert.AreEqual("Sr25519",
                wallet2.FileStore.KeyType.ToString());
            Assert.AreEqual("5Fe24e21Ff5vRtuWa4ZNPv1EGQz1zBq1VtT8ojqfmzo9k11P",
                Utils.GetAddressFrom(wallet2.FileStore.PublicKey));
            Assert.AreEqual("0xDA24A6B58BE083B58E3F011929B8A454B5FE9F1B91961DCC766D3E9F6AFE7AF96AAC1372DBA4537856F95C7E47A365C10590ACC092DB5AA95D6ECF5E06167B799AC6247178B7C51AC9B8F64C16602659",
                Utils.Bytes2HexString(wallet2.FileStore.EncryptedSeed));
            Assert.AreEqual("0xD048477FCAD42D83402CDE3B2AF369D4",
                Utils.Bytes2HexString(wallet2.FileStore.Salt));
            wallet2.Unlock("aA1234dd");
            Assert.True(wallet2.IsUnlocked);
            Assert.AreEqual("0x6BED04FEE1504A49825339A68F601F7739FA7CEBF3B5E6A4A2476979F53CF40A112F6ED717AE8E8F5134C784A07DE6F3B2F7DA51D8117C566547A5038D4B3C27",
                Utils.Bytes2HexString(wallet2.Account.PrivateKey));
        }

        [Test]
        public void CreateWalletEd25519Test()
        {
            var walletName = "wallet1";

            Wallet.CreateFromRandom("aA1234dd", KeyType.Ed25519, walletName, out Wallet wallet1);
            Assert.True(wallet1.IsStored);
            Assert.True(wallet1.IsUnlocked);

            // load wallet wallet
            Wallet.Load(walletName, out Wallet wallet2);
            Assert.True(wallet2.IsStored);
            Assert.False(wallet2.IsUnlocked);
            wallet2.Unlock("aA1234dd");
            Assert.True(wallet2.IsUnlocked);
            Assert.AreEqual(wallet1.Account.Value, wallet2.Account.Value);
        }

        [Test]
        public void CreateWalletSr25519Test()
        {
            var walletName = "wallet2";

            // create new wallet with password and persist
            Wallet.CreateFromRandom("aA1234dd", KeyType.Sr25519, walletName, out Wallet wallet1);
            Assert.True(wallet1.IsStored);
            Assert.True(wallet1.IsUnlocked);

            // read wallet
            Wallet.Load(walletName, out Wallet wallet2);
            Assert.True(wallet2.IsStored);
            Assert.False(wallet2.IsUnlocked);
            wallet2.Unlock("aA1234dd");
            Assert.True(wallet2.IsUnlocked);
            Assert.AreEqual(wallet1.Account.Value, wallet2.Account.Value);
        }

        [Test]
        public void CreateMnemonicSr25519Test()
        {
            //var mnemonic = "donor rocket find fan language damp yellow crouch attend meat hybrid pulse";
            var mnemonic = "tornado glad segment lift squirrel top ball soldier joy sudden edit advice";
            var walletName = "mnem_wallet1";

            // create new wallet with password and persist
            Wallet.CreateFromMnemonic("aA1234dd", mnemonic, KeyType.Sr25519, Mnemonic.BIP39Wordlist.English, walletName, out Wallet wallet1);
            Assert.True(wallet1.IsStored);
            Assert.True(wallet1.IsUnlocked);
            Assert.AreEqual("5Fe24e21Ff5vRtuWa4ZNPv1EGQz1zBq1VtT8ojqfmzo9k11P", wallet1.Account.Value);

            // read wallet
            Wallet.Load(walletName, out Wallet wallet2);
            Assert.True(wallet2.IsStored);
            Assert.False(wallet2.IsUnlocked);
            wallet2.Unlock("aA1234dd");
            Assert.True(wallet2.IsUnlocked);
            Assert.AreEqual(wallet1.Account.Value, wallet2.Account.Value);
        }

        [Test]
        public void CreateMnemonicEd25519Test()
        {
            var mnemonic = "tornado glad segment lift squirrel top ball soldier joy sudden edit advice";
            var walletName = "mnem_wallet2";

            // create new wallet with password and persist
            Wallet.CreateFromMnemonic("aA1234dd", mnemonic, KeyType.Ed25519, Mnemonic.BIP39Wordlist.English, walletName, out Wallet wallet1);
            Assert.True(wallet1.IsStored);
            Assert.True(wallet1.IsUnlocked);
            Assert.AreEqual("5CcaF7yE6YU67TyPHjSwd9DKiVBTAS2AktdxNG3DeLYs63gF", wallet1.Account.Value);

            // read wallet
            Wallet.Load(walletName, out Wallet wallet2);
            Assert.True(wallet2.IsStored);
            Assert.False(wallet2.IsUnlocked);
            wallet2.Unlock("aA1234dd");
            Assert.True(wallet2.IsUnlocked);
            Assert.AreEqual(wallet1.Account.Value, wallet2.Account.Value);
        }

        [Test]
        public void SignatureVerifyTest()
        {
            var data = Encoding.UTF8.GetBytes("Let's sign this message, now!");

            Random random = new Random();
            var randomBytes = new byte[16];
            random.NextBytes(randomBytes);

            var mnemonic = string.Join(" ", Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
            var accountSr = Mnemonic.GetAccountFromMnemonic(mnemonic, "", KeyType.Sr25519);

            Assert.True(Wallet.TrySignMessage(accountSr, data, out byte[] signatureSrNoWrap, false));
            Assert.True(Wallet.TrySignMessage(accountSr, data, out byte[] signatureSrWrap, true));

            Assert.True(Wallet.VerifySignature(accountSr, data, signatureSrNoWrap, false));
            Assert.True(Wallet.VerifySignature(accountSr, data, signatureSrWrap, true));

            var accountEd = Mnemonic.GetAccountFromMnemonic(mnemonic, "", KeyType.Ed25519);

            Assert.True(Wallet.TrySignMessage(accountEd, data, out byte[] signatureEdNoWrap, false));
            Assert.True(Wallet.TrySignMessage(accountEd, data, out byte[] signatureEdWrap, true));

            Assert.True(Wallet.VerifySignature(accountEd, data, signatureEdNoWrap, false));
            Assert.True(Wallet.VerifySignature(accountEd, data, signatureEdWrap, true));
        }
    }
}