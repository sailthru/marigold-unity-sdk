plugins {
    id("com.android.library")
}

android {
    compileSdk = 34
    buildToolsVersion = "34.0.0"
    namespace = "com.marigold.sdk.unity"

    defaultConfig {
        minSdk = 21
    }
    buildTypes {
        getByName("release") {
            isMinifyEnabled = false
            proguardFiles(getDefaultProguardFile("proguard-android.txt"), "proguard-rules.pro")
        }
    }
}

tasks.register<Delete>("clearJar") {
    delete("build/libs/MarigoldWrapper.jar")
}

tasks.register<Copy>("makeJar") {
    dependsOn("clearJar", "build")

    from("build/intermediates/bundles/release/")
    into("build/libs/")
    include("classes.jar")
    rename ("classes.jar", "MarigoldWrapper.jar")
}

repositories {
    maven {
        url = uri("https://github.com/carnivalmobile/maven-repository/raw/master/")
    }
}

dependencies {
    implementation(fileTree(mapOf("dir" to "libs", "include" to listOf("*.jar"))))
    implementation("com.marigold.sdk:marigold:20.+")
}