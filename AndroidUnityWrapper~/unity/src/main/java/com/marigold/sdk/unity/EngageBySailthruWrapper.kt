package com.marigold.sdk.unity

import android.net.Uri
import com.marigold.sdk.EngageBySailthru
import com.marigold.sdk.EngageBySailthru.TrackHandler
import com.marigold.sdk.Marigold
import com.marigold.sdk.model.Purchase
import com.marigold.sdk.unity.UnitySender.Companion.ENGAGE_ST_RECEIVE_LINK
import com.marigold.sdk.unity.UnitySender.Companion.ENGAGE_ST_RECEIVE_VARS
import com.marigold.sdk.unity.UnitySender.Companion.ENGAGE_ST_UNITY
import org.jetbrains.annotations.VisibleForTesting
import org.json.JSONArray
import org.json.JSONException
import org.json.JSONObject
import java.lang.IllegalArgumentException
import java.lang.reflect.Constructor
import java.lang.reflect.InvocationTargetException
import java.net.URI
import java.net.URISyntaxException

@Suppress("unused")
object EngageBySailthruWrapper {
    @VisibleForTesting
    internal lateinit var engageBySailthru: EngageBySailthru
    @VisibleForTesting
    internal var unitySender = UnitySender()

    fun trackPageview(url: String, tags: String?) {
        val uri =  try {
            URI(url)
        } catch (e: Exception) {
            unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
            return
        }

        var tagsList: List<String>? = null
        if (tags != null) {
            tagsList = getStringListFromJSONArrayString(tags)
            if (tagsList == null) return
        }

        getEngageBySailthru()?.trackPageview(uri, tagsList, object : TrackHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun trackImpression(sectionId: String, urls: String?) {
        var uris: List<URI>? = null
        if (urls != null) {
            uris = getStringListFromJSONArrayString(urls)?.map { url ->
                try {
                    URI(url)
                } catch (e: URISyntaxException) {
                    unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
                    return
                }
            }
            if (uris == null) return
        }

        getEngageBySailthru()?.trackImpression(sectionId, uris, object : TrackHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun trackClick(sectionId: String, url: String) {
        val uri =  try {
            URI(url)
        } catch (e: Exception) {
            unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
            return
        }

        getEngageBySailthru()?.trackClick(sectionId, uri, object : TrackHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun setUserEmail(userEmail: String?) {
        getEngageBySailthru()?.setUserEmail(userEmail, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun setUserId(userId: String?) {
        getEngageBySailthru()?.setUserId(userId, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun logEvent(value: String) {
        getEngageBySailthru()?.logEvent(value)
    }

    fun logEvent(value: String, varsString: String) {
        val vars = getVarsJson(varsString) ?: return
        getEngageBySailthru()?.logEvent(value, vars)
    }
    
    fun clearEvents() {
        getEngageBySailthru()?.clearEvents(object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun setProfileVars(varsString: String) {
        val vars = getVarsJson(varsString) ?: return
        getEngageBySailthru()?.setProfileVars(vars, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun getProfileVars() {
        getEngageBySailthru()?.getProfileVars(object : Marigold.MarigoldHandler<JSONObject?> {
            override fun onSuccess(value: JSONObject?) {
                value?.let { vars ->
                    unitySender.sendUnityMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_VARS, vars.toString())
                }
            }
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun handleSailthruLink(linkString: String) {
        val uri = Uri.parse(linkString)
        val unwrappedLink = try {
            val engageBySailthru = getEngageBySailthru() ?: return
            engageBySailthru.handleSailthruLink(uri, object : Marigold.MarigoldHandler<Void?> {
                override fun onSuccess(value: Void?) {}
                override fun onFailure(error: Error) {
                    unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
                }
            })
        } catch (e: IllegalArgumentException) {
            unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
            return
        }
        unitySender.sendUnityMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_LINK, unwrappedLink.toString())
    }

    fun logPurchase(purchaseString: String) {
        val purchase = getPurchase(purchaseString) ?: return
        getEngageBySailthru()?.logPurchase(purchase, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    fun logAbandonedCart(purchaseString: String) {
        val purchase = getPurchase(purchaseString) ?: return
        getEngageBySailthru()?.logAbandonedCart(purchase, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                unitySender.sendErrorMessage(ENGAGE_ST_UNITY, error)
            }
        })
    }

    private fun getVarsJson(varsString: String): JSONObject? = try {
        JSONObject(varsString)
    } catch (e: JSONException) {
        unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
        null
    }

    private fun getPurchase(purchaseString: String): Purchase? = try {
        val purchaseJson = JSONObject(purchaseString)
        val constructor: Constructor<Purchase> =
            Purchase::class.java.getDeclaredConstructor(JSONObject::class.java)
        constructor.isAccessible = true
        constructor.newInstance(purchaseJson)
    } catch (e: InvocationTargetException) {
        unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e.cause ?: Error("Error creating purchase instance"))
        null
    } catch (e: Exception) {
        unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
        null
    }

    private fun getStringListFromJSONArrayString(arrayString: String?): List<String>? {
        arrayString ?: return null

        return try {
            val list = mutableListOf<String>()
            val jsonArray = JSONArray(arrayString)
            for (i in 0 until jsonArray.length()) {
                list.add(jsonArray.getString(i))
            }
            list
        } catch (e: Exception) {
            unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
            null
        }
    }

    private fun getEngageBySailthru(): EngageBySailthru? {
        if (this::engageBySailthru.isInitialized) {
            return engageBySailthru
        }

        return try {
            engageBySailthru = EngageBySailthru()
            engageBySailthru
        } catch (e: Exception) {
            unitySender.sendErrorMessage(ENGAGE_ST_UNITY, e)
            null
        }
    }
}