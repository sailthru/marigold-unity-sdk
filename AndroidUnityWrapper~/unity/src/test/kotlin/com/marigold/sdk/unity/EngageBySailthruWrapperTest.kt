package com.marigold.sdk.unity

import android.net.Uri
import com.marigold.sdk.EngageBySailthru
import com.marigold.sdk.Marigold
import com.marigold.sdk.model.Purchase
import com.marigold.sdk.unity.UnitySender.Companion.ENGAGE_ST_RECEIVE_LINK
import com.marigold.sdk.unity.UnitySender.Companion.ENGAGE_ST_RECEIVE_VARS
import com.marigold.sdk.unity.UnitySender.Companion.ENGAGE_ST_UNITY
import com.unity3d.player.UnityPlayer
import org.json.JSONException
import org.json.JSONObject
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.ArgumentCaptor
import org.mockito.Captor
import org.mockito.Mock
import org.mockito.MockitoAnnotations
import org.mockito.kotlin.any
import org.mockito.kotlin.capture
import org.mockito.kotlin.doReturn
import org.mockito.kotlin.doThrow
import org.mockito.kotlin.eq
import org.mockito.kotlin.isNull
import org.mockito.kotlin.never
import org.mockito.kotlin.verify
import org.mockito.kotlin.verifyNoInteractions
import org.mockito.kotlin.whenever
import org.robolectric.RobolectricTestRunner
import java.lang.Error
import java.net.URI
import java.net.URISyntaxException

@RunWith(RobolectricTestRunner::class)
class EngageBySailthruWrapperTest {
    @Mock
    private lateinit var engageBySailthru: EngageBySailthru
    @Mock
    private lateinit var unitySender: UnitySender

    @Captor
    private lateinit var trackHandlerCaptor: ArgumentCaptor<EngageBySailthru.TrackHandler>
    @Captor
    private lateinit var marigoldVoidHandlerCaptor: ArgumentCaptor<Marigold.MarigoldHandler<Void?>>
    @Captor
    private lateinit var marigoldJsonHandlerCaptor: ArgumentCaptor<Marigold.MarigoldHandler<JSONObject?>>
    @Captor
    private lateinit var throwableCaptor: ArgumentCaptor<Throwable>
    @Captor
    private lateinit var jsonCaptor: ArgumentCaptor<JSONObject>
    @Captor
    private lateinit var purchaseCaptor: ArgumentCaptor<Purchase>

    private val error = Error("Test Error")
    private val urlString = "https://www.sailthru.com"
    private val tagsArrayString = "[\"tag1\",\"tag2\"]"
    private val testString = "test me"
    private val urlsArrayString = "[\"https://www.sailthru.com\",\"https://www.sailthru.com/something\"]"
    private val jsonString = "{\"key\":\"value\"}"
    private val invalidJsonString = "{]"
    private val stLinkString = "https://link.varickandvandam.com/click/5afcdea395a7a1540e04604d/aHR0cHM6Ly92YXJpY2thbmR2YW5kYW0uY29tL3Byb2R1Y3RzLzEwODgzNTg/5a7cbd790aea1153738b60f3B81a4ee8d"
    private val stLinkUnwrappedString = "https://varickandvandam.com/products/1088358"
    private val purchaseJsonString = "{\"items\":[{\"qty\":2,\"title\":\"item name\",\"price\":1234,\"id\":\"2345\",\"url\":\"https://www.sailthru.com\"}]}"

    @Before
    fun setup() {
        MockitoAnnotations.openMocks(this)
        EngageBySailthruWrapper.engageBySailthru = engageBySailthru
        EngageBySailthruWrapper.unitySender = unitySender
    }

    @After
    fun tearDown() {
        UnityPlayer.currentActivity = null
    }

    @Test
    fun `test trackPageview with success response`() {
        EngageBySailthruWrapper.trackPageview(urlString, tagsArrayString)
        val expectedUri = URI(urlString)
        val expectedTags = arrayListOf("tag1", "tag2")
        verify(engageBySailthru).trackPageview(eq(expectedUri), eq(expectedTags), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test trackPageview with null tags and success response`() {
        EngageBySailthruWrapper.trackPageview(urlString, null)
        val expectedUri = URI(urlString)
        verify(engageBySailthru).trackPageview(eq(expectedUri), isNull(), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test trackPageview with error response`() {
        EngageBySailthruWrapper.trackPageview(urlString, tagsArrayString)
        val expectedUri = URI(urlString)
        val expectedTags = arrayListOf("tag1", "tag2")
        verify(engageBySailthru).trackPageview(eq(expectedUri), eq(expectedTags), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test trackPageview with invalid url`() {
        EngageBySailthruWrapper.trackPageview("$$##L.//sa1e", tagsArrayString)
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is URISyntaxException)
    }

    @Test
    fun `test trackPageview with invalid tags JSON`() {
        EngageBySailthruWrapper.trackPageview(urlString, invalidJsonString)
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is JSONException)
    }

    @Test
    fun `test trackImpression with success response`() {
        EngageBySailthruWrapper.trackImpression(testString, urlsArrayString)
        val expectedUrls = arrayListOf(URI("https://www.sailthru.com"), URI("https://www.sailthru.com/something"))
        verify(engageBySailthru).trackImpression(eq(testString), eq(expectedUrls), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test trackImpression with null urls and success response`() {
        EngageBySailthruWrapper.trackImpression(testString, null)
        verify(engageBySailthru).trackImpression(eq(testString), isNull(), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test trackImpression with error response`() {
        EngageBySailthruWrapper.trackImpression(testString, urlsArrayString)
        val expectedUrls = arrayListOf(URI("https://www.sailthru.com"), URI("https://www.sailthru.com/something"))
        verify(engageBySailthru).trackImpression(eq(testString), eq(expectedUrls), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test trackImpression with invalid urls`() {
        EngageBySailthruWrapper.trackImpression(testString, "[\"$$##L.//sa1e\", \"$$##L.//sa1e\"]")
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is URISyntaxException)
    }

    @Test
    fun `test trackImpression with invalid urls JSON`() {
        EngageBySailthruWrapper.trackImpression(testString, invalidJsonString)
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is JSONException)
    }

    @Test
    fun `test trackClick with success response`() {
        EngageBySailthruWrapper.trackClick(testString, urlString)
        val expectedUri = URI(urlString)
        verify(engageBySailthru).trackClick(eq(testString), eq(expectedUri), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onSuccess()

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test trackClick with error response`() {
        EngageBySailthruWrapper.trackClick(testString, urlString)
        val expectedUri = URI(urlString)
        verify(engageBySailthru).trackClick(eq(testString), eq(expectedUri), capture(trackHandlerCaptor))

        val handler = trackHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test trackClick with invalid url`() {
        EngageBySailthruWrapper.trackClick(testString, "$$##L.//sa1e")
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is URISyntaxException)
    }

    @Test
    fun `test setUserEmail with success response`() {
        EngageBySailthruWrapper.setUserEmail(testString)
        verify(engageBySailthru).setUserEmail(eq(testString), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test setUserEmail null with success response`() {
        EngageBySailthruWrapper.setUserEmail(null)
        verify(engageBySailthru).setUserEmail(isNull(), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test setUserEmail with error response`() {
        EngageBySailthruWrapper.setUserEmail(testString)
        verify(engageBySailthru).setUserEmail(eq(testString), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test setUserId with success response`() {
        EngageBySailthruWrapper.setUserId(testString)
        verify(engageBySailthru).setUserId(eq(testString), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test setUserId null with success response`() {
        EngageBySailthruWrapper.setUserId(null)
        verify(engageBySailthru).setUserId(isNull(), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test setUserId with error response`() {
        EngageBySailthruWrapper.setUserId(testString)
        verify(engageBySailthru).setUserId(eq(testString), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test logEvent`() {
        EngageBySailthruWrapper.logEvent(testString)
        verify(engageBySailthru).logEvent(testString)
    }

    @Test
    fun `test logEvent with vars`() {
        EngageBySailthruWrapper.logEvent(testString, jsonString)
        verify(engageBySailthru).logEvent(eq(testString), capture(jsonCaptor))

        assertEquals("value", jsonCaptor.value.getString("key"))
    }

    @Test
    fun `test logEvent with invalid vars JSON`() {
        EngageBySailthruWrapper.logEvent(testString, invalidJsonString)
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is JSONException)
    }

    @Test
    fun `test clearEvents with success response`() {
        EngageBySailthruWrapper.clearEvents()
        verify(engageBySailthru).clearEvents(capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test clearEvents with error response`() {
        EngageBySailthruWrapper.clearEvents()
        verify(engageBySailthru).clearEvents(capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test setProfileVars with success response`() {
        EngageBySailthruWrapper.setProfileVars(jsonString)
        verify(engageBySailthru).setProfileVars(capture(jsonCaptor), capture(marigoldVoidHandlerCaptor))

        assertEquals("value", jsonCaptor.value.getString("key"))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test setProfileVars with error response`() {
        EngageBySailthruWrapper.setProfileVars(jsonString)
        verify(engageBySailthru).setProfileVars(capture(jsonCaptor), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test setProfileVars with invalid JSON`() {
        EngageBySailthruWrapper.setProfileVars(invalidJsonString)
        verifyNoInteractions(engageBySailthru)

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is JSONException)
    }

    @Test
    fun `test getProfileVars with success response`() {
        EngageBySailthruWrapper.getProfileVars()
        verify(engageBySailthru).getProfileVars(capture(marigoldJsonHandlerCaptor))

        val handler = marigoldJsonHandlerCaptor.value
        val jsonObject = JSONObject(jsonString)
        handler.onSuccess(jsonObject)

        verify(unitySender).sendUnityMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_VARS, jsonString)
    }

    @Test
    fun `test getProfileVars with success response but null vars`() {
        EngageBySailthruWrapper.getProfileVars()
        verify(engageBySailthru).getProfileVars(capture(marigoldJsonHandlerCaptor))

        val handler = marigoldJsonHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test getProfileVars with error response`() {
        EngageBySailthruWrapper.getProfileVars()
        verify(engageBySailthru).getProfileVars(capture(marigoldJsonHandlerCaptor))

        val handler = marigoldJsonHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test handleSailthruLink with success response`() {
        val uri = Uri.parse(stLinkString)
        val unwrappedUri = Uri.parse(stLinkUnwrappedString)
        doReturn(unwrappedUri).whenever(engageBySailthru).handleSailthruLink(any(), any())

        EngageBySailthruWrapper.handleSailthruLink(stLinkString)
        verify(engageBySailthru).handleSailthruLink(eq(uri), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verify(unitySender).sendUnityMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_LINK, stLinkUnwrappedString)
    }

    @Test
    fun `test handleSailthruLink with error response`() {
        val uri = Uri.parse(stLinkString)
        val unwrappedUri = Uri.parse(stLinkUnwrappedString)
        doReturn(unwrappedUri).whenever(engageBySailthru).handleSailthruLink(any(), any())

        EngageBySailthruWrapper.handleSailthruLink(stLinkString)
        verify(engageBySailthru).handleSailthruLink(eq(uri), capture(marigoldVoidHandlerCaptor))

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
        verify(unitySender).sendUnityMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_LINK, stLinkUnwrappedString)
    }

    @Test
    fun `test handleSailthruLink with invalid link`() {
        val uri = Uri.parse(stLinkString)
        val exception = IllegalArgumentException()
        doThrow(exception).whenever(engageBySailthru).handleSailthruLink(any(), any())

        EngageBySailthruWrapper.handleSailthruLink(stLinkString)
        verify(engageBySailthru).handleSailthruLink(eq(uri), any())

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, exception)
        verify(unitySender, never()).sendUnityMessage(any(), any(), any())
    }

    @Test
    fun `test logPurchase with success response`() {
        EngageBySailthruWrapper.logPurchase(purchaseJsonString)
        verify(engageBySailthru).logPurchase(capture(purchaseCaptor), capture(marigoldVoidHandlerCaptor))

        val purchase = purchaseCaptor.value
        val item = purchase.purchaseItems.first()
        assertEquals(2, item.quantity)
        assertEquals("item name", item.title)
        assertEquals(1234, item.price)
        assertEquals("2345", item.ID)
        assertEquals(URI("https://www.sailthru.com"), item.url)

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test logPurchase with error response`() {
        EngageBySailthruWrapper.logPurchase(purchaseJsonString)
        verify(engageBySailthru).logPurchase(capture(purchaseCaptor), capture(marigoldVoidHandlerCaptor))

        val purchase = purchaseCaptor.value
        val item = purchase.purchaseItems.first()
        assertEquals(2, item.quantity)
        assertEquals("item name", item.title)
        assertEquals(1234, item.price)
        assertEquals("2345", item.ID)
        assertEquals(URI("https://www.sailthru.com"), item.url)

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test logPurchase with invalid JSON`() {
        EngageBySailthruWrapper.logPurchase(invalidJsonString)
        verify(engageBySailthru, never()).logPurchase(any(), any())

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is JSONException)
    }

    @Test
    fun `test logAbandonedCart with success response`() {
        EngageBySailthruWrapper.logAbandonedCart(purchaseJsonString)
        verify(engageBySailthru).logAbandonedCart(capture(purchaseCaptor), capture(marigoldVoidHandlerCaptor))

        val purchase = purchaseCaptor.value
        val item = purchase.purchaseItems.first()
        assertEquals(2, item.quantity)
        assertEquals("item name", item.title)
        assertEquals(1234, item.price)
        assertEquals("2345", item.ID)
        assertEquals(URI("https://www.sailthru.com"), item.url)

        val handler = marigoldVoidHandlerCaptor.value
        handler.onSuccess(null)

        verifyNoInteractions(unitySender)
    }

    @Test
    fun `test logAbandonedCart with error response`() {
        EngageBySailthruWrapper.logAbandonedCart(purchaseJsonString)
        verify(engageBySailthru).logAbandonedCart(capture(purchaseCaptor), capture(marigoldVoidHandlerCaptor))

        val purchase = purchaseCaptor.value
        val item = purchase.purchaseItems.first()
        assertEquals(2, item.quantity)
        assertEquals("item name", item.title)
        assertEquals(1234, item.price)
        assertEquals("2345", item.ID)
        assertEquals(URI("https://www.sailthru.com"), item.url)

        val handler = marigoldVoidHandlerCaptor.value
        handler.onFailure(error)

        verify(unitySender).sendErrorMessage(ENGAGE_ST_UNITY, error)
    }

    @Test
    fun `test logAbandonedCart with invalid JSON`() {
        EngageBySailthruWrapper.logAbandonedCart(invalidJsonString)
        verify(engageBySailthru, never()).logAbandonedCart(any(), any())

        verify(unitySender).sendErrorMessage(eq(ENGAGE_ST_UNITY), capture(throwableCaptor))
        assertTrue(throwableCaptor.value is JSONException)
    }
}