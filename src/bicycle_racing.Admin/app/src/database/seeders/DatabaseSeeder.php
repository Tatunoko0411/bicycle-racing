<?php

namespace Database\Seeders;

use App\Models\User;

// use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     */
    public function run(): void
    {
        // User::factory(10)->create();

        User::factory()->create([
            'name' => 'aaa',
            'token' => 'aaa',
        ]);
        User::factory()->create([
            'name' => 'bbb',
            'token' => 'bbb',

        ]);
        User::factory()->create([
            'name' => 'ccc',
            'token' => 'ccc',
        ]);
    }
}
