// Copyright (C) 2023 Search Pioneer - https://www.searchpioneer.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//         http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;

namespace SearchPioneer.Weaviate.Client.IntegrationTests.Api.Backup;

[Collection("Sequential")]
public class BackupTests : TestBase
{
	private const string BackupsDirectory = "/tmp/backups";

	[Fact]
	public void CreateAndRestoreBackupWithWaiting()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.True(backup.HttpStatusCode == 200);
		Assert.Contains(CLASS_NAME_PIZZA, backup.Result!.Classes!);
		Assert.Equal(backupId, backup.Result.Id);
		Assert.StartsWith(BackupsDirectory, backup.Result.Path);
		Assert.Equal(BackupStatus.Success, backup.Result.Status);
		Assert.Equal(Backend.Filesystem, backup.Result.Backend);

		AssertAllPizzasExist();

		var backupStatus = Client.Backup.Status(new(backupId, Backend.Filesystem));
		Assert.True(backupStatus.HttpStatusCode == 200);
		Assert.Equal(backupId, backupStatus.Result!.Id);
		Assert.StartsWith(BackupsDirectory, backupStatus.Result.Path);
		Assert.Equal(BackupStatus.Success, backupStatus.Result.Status);
		Assert.Equal(Backend.Filesystem, backupStatus.Result.Backend);

		DeleteAllPizzas();

		var restore = Client.Backup.Restore(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.True(restore.HttpStatusCode == 200);
		Assert.Equal(backupId, restore.Result!.Id);
		Assert.StartsWith(BackupsDirectory, restore.Result.Path);
		Assert.Equal(BackupStatus.Success, restore.Result.Status);
		Assert.Equal(Backend.Filesystem, restore.Result.Backend);

		AssertAllPizzasExist();

		var restoreStatus = Client.Backup.RestoreStatus(new(backupId, Backend.Filesystem));
		Assert.True(restoreStatus.HttpStatusCode == 200);
		Assert.Equal(backupId, restoreStatus.Result!.Id);
		Assert.StartsWith(BackupsDirectory, restoreStatus.Result.Path);
		Assert.Equal(BackupStatus.Success, restoreStatus.Result.Status);
		Assert.Equal(Backend.Filesystem, restoreStatus.Result.Backend);
	}

	[Fact]
	public void CreateAndRestoreBackupWithoutWaiting()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }
		});

		Assert.True(backup.HttpStatusCode == 200);
		Assert.Contains(CLASS_NAME_PIZZA, backup.Result!.Classes!);
		Assert.Equal(backupId, backup.Result.Id);
		Assert.StartsWith(BackupsDirectory, backup.Result.Path);
		Assert.Equal(BackupStatus.Started, backup.Result.Status);
		Assert.Equal(Backend.Filesystem, backup.Result.Backend);

		while (true)
		{
			var backupStatus = Client.Backup.Status(new(backupId, Backend.Filesystem));
			Assert.True(backupStatus.HttpStatusCode == 200);
			Assert.Equal(backupId, backupStatus.Result!.Id);
			Assert.StartsWith(BackupsDirectory, backupStatus.Result.Path);
			Assert.Equal(Backend.Filesystem, backupStatus.Result.Backend);

			if (backupStatus.Result.Status == BackupStatus.Success)
				break;

			Thread.Sleep(100);
		}

		AssertAllPizzasExist();

		DeleteAllPizzas();

		var restore = Client.Backup.Restore(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }
		});

		Assert.True(restore.HttpStatusCode == 200);
		Assert.Equal(backupId, restore.Result!.Id);
		Assert.StartsWith(BackupsDirectory, restore.Result.Path);
		Assert.Equal(BackupStatus.Started, restore.Result.Status);
		Assert.Equal(Backend.Filesystem, restore.Result.Backend);

		while (true)
		{
			var restoreStatus = Client.Backup.RestoreStatus(new(backupId, Backend.Filesystem));
			Assert.True(restoreStatus.HttpStatusCode == 200);
			Assert.Equal(backupId, restoreStatus.Result!.Id);
			Assert.StartsWith(BackupsDirectory, restoreStatus.Result.Path);
			Assert.Equal(Backend.Filesystem, restoreStatus.Result.Backend);

			if (restoreStatus.Result.Status == BackupStatus.Success)
				break;

			Thread.Sleep(100);
		}

		AssertAllPizzasExist();
	}

	[Fact]
	public void CreateAndRestore1Of2Classes()
	{
		CreateTestSchemaAndData(Client);

		AssertAllPizzasExist();
		AssertAllSoupsExist();

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem) { WaitForCompletion = true });

		Assert.True(backup.HttpStatusCode == 200);
		Assert.Contains(CLASS_NAME_PIZZA, backup.Result!.Classes!);
		Assert.Equal(backupId, backup.Result.Id);
		Assert.StartsWith(BackupsDirectory, backup.Result.Path);
		Assert.Equal(BackupStatus.Success, backup.Result.Status);
		Assert.Equal(Backend.Filesystem, backup.Result.Backend);

		var backupStatus = Client.Backup.Status(new(backupId, Backend.Filesystem));
		Assert.True(backupStatus.HttpStatusCode == 200);
		Assert.Equal(backupId, backupStatus.Result!.Id);
		Assert.StartsWith(BackupsDirectory, backupStatus.Result.Path);
		Assert.Equal(BackupStatus.Success, backupStatus.Result.Status);
		Assert.Equal(Backend.Filesystem, backupStatus.Result.Backend);

		DeleteAllPizzas();

		var restore = Client.Backup.Restore(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.True(restore.HttpStatusCode == 200);
		Assert.Equal(backupId, restore.Result!.Id);
		Assert.StartsWith(BackupsDirectory, restore.Result.Path);
		Assert.Equal(BackupStatus.Success, restore.Result.Status);
		Assert.Equal(Backend.Filesystem, restore.Result.Backend);

		AssertAllPizzasExist();
		AssertAllSoupsExist();

		var restoreStatus = Client.Backup.RestoreStatus(new(backupId, Backend.Filesystem));
		Assert.True(restoreStatus.HttpStatusCode == 200);
		Assert.Equal(backupId, restoreStatus.Result!.Id);
		Assert.StartsWith(BackupsDirectory, restoreStatus.Result.Path);
		Assert.Equal(BackupStatus.Success, restoreStatus.Result.Status);
		Assert.Equal(Backend.Filesystem, restoreStatus.Result.Backend);
	}

	[Fact]
	public void FailOnCreateBackupForNotExistingClass()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { "not-exist" }, WaitForCompletion = true
		});

		Assert.Equal(422, backup.HttpStatusCode);
		Assert.Contains("not-exist", backup.Error!.Error!.First().Message);
	}

	[Fact]
	public void FailOnRestoreBackupForExistingClass()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.True(backup.HttpStatusCode == 200);
		Assert.Contains(CLASS_NAME_PIZZA, backup.Result!.Classes!);
		Assert.Equal(backupId, backup.Result.Id);
		Assert.StartsWith(BackupsDirectory, backup.Result.Path);
		Assert.Equal(BackupStatus.Success, backup.Result.Status);
		Assert.Equal(Backend.Filesystem, backup.Result.Backend);

		var restore = Client.Backup.Restore(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.Equal(backupId, restore.Result!.Id);
		Assert.StartsWith(BackupsDirectory, restore.Result.Path);
		Assert.Equal(Backend.Filesystem, restore.Result.Backend);
		Assert.Equal(BackupStatus.Failed, restore.Result.Status);
		Assert.Contains("restore class Pizza: already exists", restore.Result.Error);
	}

	[Fact]
	public void FailOnCreateOfExistingBackup()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.True(backup.HttpStatusCode == 200);
		Assert.Contains(CLASS_NAME_PIZZA, backup.Result!.Classes!);
		Assert.Equal(backupId, backup.Result.Id);
		Assert.StartsWith(BackupsDirectory, backup.Result.Path);
		Assert.Equal(BackupStatus.Success, backup.Result.Status);
		Assert.Equal(Backend.Filesystem, backup.Result.Backend);

		var backup2 = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.Equal(422, backup2.HttpStatusCode);
		Assert.Contains(backupId, backup2.Error!.Error!.First().Message);
	}

	[Fact]
	public void FailOnCreateStatusOfNotExistingBackup()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backupStatus = Client.Backup.Status(new(backupId, Backend.Filesystem));
		Assert.True(backupStatus.HttpStatusCode == 404);
		Assert.Contains(backupId, backupStatus.Error!.Error!.First().Message);
	}

	[Fact]
	public void FailOnRestoreOfNotExistingBackup()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "not-existing-backup-id";
		var restore = Client.Backup.Restore(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA }, WaitForCompletion = true
		});

		Assert.Equal(404, restore.HttpStatusCode);
		Assert.Contains(backupId, restore.Error!.Error!.First().Message);
	}

	[Fact]
	public void FailOnCreateBackupForBothIncludeAndExcludeClasses()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA },
			ExcludeClasses = new[] { CLASS_NAME_SOUP },
			WaitForCompletion = true
		});

		Assert.Equal(422, backup.HttpStatusCode);
		Assert.Contains("include", backup.Error!.Error!.First().Message);
		Assert.Contains("exclude", backup.Error!.Error!.First().Message);
	}

	[Fact]
	public void FailOnRestoreBackupForBothIncludeAndExcludeClasses()
	{
		CreateTestSchemaAndData(Client);

		var backupId = "backup-" + new Random().Next(int.MaxValue);

		var backup = Client.Backup.Create(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA, CLASS_NAME_SOUP }, WaitForCompletion = true
		});

		Assert.True(backup.HttpStatusCode == 200);
		Assert.Contains(CLASS_NAME_PIZZA, backup.Result!.Classes!);
		Assert.Contains(CLASS_NAME_SOUP, backup.Result.Classes);
		Assert.Equal(backupId, backup.Result.Id);
		Assert.StartsWith(BackupsDirectory, backup.Result.Path);
		Assert.Equal(BackupStatus.Success, backup.Result.Status);
		Assert.Equal(Backend.Filesystem, backup.Result.Backend);

		DeleteAllPizzas();

		var restore = Client.Backup.Restore(new(backupId, Backend.Filesystem)
		{
			IncludeClasses = new[] { CLASS_NAME_PIZZA },
			ExcludeClasses = new[] { CLASS_NAME_SOUP },
			WaitForCompletion = true
		});

		Assert.Equal(422, restore.HttpStatusCode);
		Assert.Contains("include", restore.Error!.Error!.First().Message);
		Assert.Contains("exclude", restore.Error!.Error!.First().Message);
	}

	private void DeleteAllPizzas()
	{
		var delete = Client.Schema.DeleteClass(new(CLASS_NAME_PIZZA));
		Assert.True(delete.HttpStatusCode == 200);
	}

	private void AssertAllPizzasExist()
	{
		var pizzas = Client.Data.Get(new() { Class = CLASS_NAME_PIZZA });
		Assert.True(pizzas.HttpStatusCode == 200);
		Assert.Equal(4, pizzas.Result!.Length);
	}

	private void AssertAllSoupsExist()
	{
		var soups = Client.Data.Get(new() { Class = CLASS_NAME_SOUP });
		Assert.True(soups.HttpStatusCode == 200);
		Assert.Equal(2, soups.Result!.Length);
	}
}
