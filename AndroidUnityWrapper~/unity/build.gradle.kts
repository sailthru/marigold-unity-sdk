plugins {
    id("com.android.library")
    id("org.jetbrains.kotlin.android")
}

android {
    compileSdk = 35
    buildToolsVersion = "35.0.0"
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
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    kotlinOptions {
        jvmTarget = "17"
    }
    testOptions {
        unitTests.apply {
            isReturnDefaultValues = true
        }
    }
}

tasks.register<Delete>("clearJar") {
    delete("../../Plugins/Android/libs/MarigoldWrapper.jar")
}

tasks.register<Copy>("makeJar") {
    dependsOn("clearJar", "build")

    from("build/intermediates/aar_main_jar/release/syncReleaseLibJars")
    into("../../Plugins/Android/libs/")
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
    implementation("com.marigold.sdk:marigold:24.0.0")
    implementation("androidx.core:core-ktx:1.13.1")

    testImplementation("junit:junit:4.13.2")
    testImplementation("org.mockito:mockito-inline:5.2.0")
    testImplementation("org.mockito.kotlin:mockito-kotlin:5.3.1")
    testImplementation("org.robolectric:robolectric:4.12.1")
    testImplementation("androidx.test:core:1.6.1")
}