# Cumulativerse Substrate Wallet (NETStandard2.0, NETStandard2.1, NET6.0)
*Cumulativerse Substrate Wallet functions for substrate-based nodes*  
![Build](https://github.com/Cumulativerse/substrate-wallet-dotnet/actions/workflows/build.yml/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/Cumulativerse.Substrate.Wallet)](https://www.nuget.org/packages/Cumulativerse.Substrate.Wallet/)
[![GitHub issues](https://img.shields.io/github/issues/Cumulativerse/substrate-wallet-dotnet.svg)](https://github.com/Cumulativerse/substrate-wallet-dotnet/issues)
[![license](https://img.shields.io/github/license/Cumulativerse/substrate-wallet-dotnet)](https://github.com/Cumulativerse/substrate-wallet-dotnet/blob/origin/LICENSE)
[![contributors](https://img.shields.io/github/contributors/Cumulativerse/substrate-wallet-dotnet)](https://github.com/Cumulativerse/substrate-wallet-dotnet/graphs/contributors)  
 
## How to use ?

### Create a new Keyring

```c#
// Create a new Keyring, by default ss58 format is 42 (Substrate standard address)
var keyring = new Substrate.NET.Wallet.Keyring.Keyring();

// You can specify ss58 address if needed (check SS58 regitry here : https://github.com/paritytech/ss58-registry/blob/main/ss58-registry.json)
keyring.Ss58Format = 0; // Polkadot
```

### Create a new account with a random mnemonic phrase

```c#
// Generate a new mnemonic for a new account
var newMnemonic = Mnemonic.GenerateMnemonic(MnemonicSize.Words12);
// Also available for 15, 18, 21, 24 mnemonic words
```

### Load an account with a mnemonic phrase

```c#
// Use an existing mnemonic
var existingMnemonicAccount = "entire material egg meadow latin bargain dutch coral blood melt acoustic thought";

// Import an account from mnemonic automatically unlock all feature
var firstWallet = keyring.AddFromMnemonic(existingMnemonicAccount, new Meta() { name = "My account name"}, NetApi.Model.Types.KeyType.Ed25519);
// Account is unlock and ready to sign transaction
```

### Export an account to a file

```c#
// Choose a password to protect your account file
var walletFile = firstWallet.ToWalletFile("MyWallet", "myPassword")
var json = firstWallet.ToJson("MyWallet", "myPassword");
// Now save
```
### Import a wallet from a json file

```c#
var secondWallet = keyring.AddFromJson(json);
// Wallet is imported but locked

// You need to unlock the account with the associated password
secondWallet.Unlock("myPassword");
```
### Sign and verify with you account

```c#
string message = "Hello !";
var signature = firstWallet.Sign(message);
var isVerify = firstWallet.Verify(signature, message);
```