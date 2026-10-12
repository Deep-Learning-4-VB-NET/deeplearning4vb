Imports System
Imports NTPUDPClient = org.apache.commons.net.ntp.NTPUDPClient
Imports TimeInfo = org.apache.commons.net.ntp.TimeInfo
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.spark.time


	Public Class NTPTimeSource
		Implements TimeSource

		''' @deprecated Use <seealso cref="DL4JSystemProperties.NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY"/> 
		<Obsolete("Use <seealso cref=""DL4JSystemProperties.NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY""/>")>
		Public Const NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY As String = DL4JSystemProperties.NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY
		''' @deprecated Use <seealso cref="DL4JSystemProperties.NTP_SOURCE_SERVER_PROPERTY"/> 
		<Obsolete("Use <seealso cref=""DL4JSystemProperties.NTP_SOURCE_SERVER_PROPERTY""/>")>
		Public Const NTP_SOURCE_SERVER_PROPERTY As String = DL4JSystemProperties.NTP_SOURCE_SERVER_PROPERTY
		Public Const MAX_QUERY_RETRIES As Integer = 10
		Public Const DEFAULT_NTP_TIMEOUT_MS As Integer = 10000
		Public Shared ReadOnly DEFAULT_UPDATE_FREQUENCY As Long = 30 * 60 * 1000L '30 Minutes
		Public Const MIN_UPDATE_FREQUENCY As Long = 30000L '30 sec

		Public Const DEFAULT_NTP_SERVER As String = "0.pool.ntp.org"

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(NTPTimeSource))
'JAVA TO VB CONVERTER NOTE: The field instance was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared instance_Conflict As NTPTimeSource

		Public Shared ReadOnly Property Instance As TimeSource
			Get
				SyncLock GetType(NTPTimeSource)
					If instance_Conflict Is Nothing Then
						instance_Conflict = New NTPTimeSource()
					End If
					Return instance_Conflict
				End SyncLock
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile long lastOffsetGetTimeSystemMS = -1;
		Private lastOffsetGetTimeSystemMS As Long = -1
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile long lastOffsetMilliseconds;
		Private lastOffsetMilliseconds As Long

		Private ReadOnly synchronizationFreqMS As Long
		Private ReadOnly ntpServer As String

		Private Sub New()
			Me.New(UpdateFrequencyConfiguration, ServerConfiguration)
		End Sub

		Private Sub New(ByVal synchronizationFreqMS As Long, ByVal ntpServer As String)
			Me.synchronizationFreqMS = synchronizationFreqMS
			Me.ntpServer = ntpServer

			log.debug("Initializing NTPTimeSource with query frequency {} ms using server {}", synchronizationFreqMS, ntpServer)

			queryServerNow()

			'Start a Timer to periodically query the server
			Dim timer As New Timer(True)
			timer.scheduleAtFixedRate(New QueryServerTask(Me), synchronizationFreqMS, synchronizationFreqMS)

			log.debug("Initialized NTPTimeSource with query frequency {} ms using server {}", synchronizationFreqMS, ntpServer)
		End Sub

		'Query and parse the system property
		Private Shared ReadOnly Property UpdateFrequencyConfiguration As Long
			Get
				Dim [property] As String = System.getProperty(DL4JSystemProperties.NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY)
				Dim parseAttempt As Long? = Nothing
				Dim updateFreq As Long
				If [property] IsNot Nothing Then
					Try
						parseAttempt = Long.Parse([property])
					Catch e As Exception
						log.info("Error parsing system property ""{}"" with value ""{}""", DL4JSystemProperties.NTP_SOURCE_UPDATE_FREQUENCY_MS_PROPERTY, [property])
					End Try
					If parseAttempt IsNot Nothing Then
						If parseAttempt < MIN_UPDATE_FREQUENCY Then
							log.info("Invalid update frequency (milliseconds): {} is less than minimum {}. Using default update frequency: {} ms", parseAttempt, MIN_UPDATE_FREQUENCY, DEFAULT_UPDATE_FREQUENCY)
							updateFreq = DEFAULT_UPDATE_FREQUENCY
						Else
							updateFreq = parseAttempt
						End If
					Else
						updateFreq = DEFAULT_UPDATE_FREQUENCY
					End If
				Else
					updateFreq = DEFAULT_UPDATE_FREQUENCY
				End If
				Return updateFreq
			End Get
		End Property

		Private Shared ReadOnly Property ServerConfiguration As String
			Get
				Return System.getProperty(DL4JSystemProperties.NTP_SOURCE_SERVER_PROPERTY, DEFAULT_NTP_SERVER)
			End Get
		End Property


		Private Sub queryServerNow()
			Dim offsetResult As Long? = Nothing
			For i As Integer = 0 To MAX_QUERY_RETRIES - 1
				Try
					Dim client As New NTPUDPClient()
					client.setDefaultTimeout(DEFAULT_NTP_TIMEOUT_MS) ' Timeout if a response takes longer than 10 seconds

					client.open()
					Dim address As InetAddress = InetAddress.getByName(ntpServer)
					Dim info As TimeInfo = client.getTime(address)
					info.computeDetails()
					Dim offset As Long? = info.getOffset()
					If offset Is Nothing Then
						Throw New Exception("Could not calculate time offset (offset is null)")
					Else
						offsetResult = offset
						Exit For
					End If
				Catch e As Exception
					log.error("Error querying NTP server, attempt {} of {}", (i + 1), MAX_QUERY_RETRIES, e)
				End Try
			Next i

			If offsetResult Is Nothing Then
				log.error("Could not successfully query NTP server after " & MAX_QUERY_RETRIES & " tries")
				Throw New Exception("Could not successfully query NTP server after " & MAX_QUERY_RETRIES & " tries")
			End If

			lastOffsetGetTimeSystemMS = DateTimeHelper.CurrentUnixTimeMillis()
			lastOffsetMilliseconds = offsetResult
			log.debug("Updated local time offset based on NTP server result. Offset = {}", lastOffsetMilliseconds)
		End Sub

		'Timer task to be run periodically
		Private Class QueryServerTask
			Inherits TimerTask

			Private ReadOnly outerInstance As NTPTimeSource

			Public Sub New(ByVal outerInstance As NTPTimeSource)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub run()
				outerInstance.queryServerNow()
			End Sub
		End Class



		'Get system offset. Note: positive offset means system clock is behind time server; negative offset means system
		' clock is ahead of time server
		Private ReadOnly Property SystemOffset As Long
			Get
				SyncLock Me
					Return lastOffsetMilliseconds
				End SyncLock
			End Get
		End Property

		Public Overridable Function currentTimeMillis() As Long Implements TimeSource.currentTimeMillis
			Dim offset As Long = SystemOffset
			Dim systemTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Return systemTime + offset
		End Function
	End Class

End Namespace