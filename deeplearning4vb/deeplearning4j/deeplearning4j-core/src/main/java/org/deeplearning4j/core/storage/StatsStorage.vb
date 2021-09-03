Imports System.Collections.Generic

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

Namespace org.deeplearning4j.core.storage


	Public Interface StatsStorage
		Inherits StatsStorageRouter


		''' <summary>
		''' Close any open resources (files, etc)
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void close() throws java.io.IOException;
		Sub close()

		''' <returns> Whether the StatsStorage implementation has been closed or not </returns>
		ReadOnly Property Closed As Boolean

		''' <summary>
		''' Get a list of all sessions stored by this storage backend
		''' </summary>
		Function listSessionIDs() As IList(Of String)

		''' <summary>
		''' Check if the specified session ID exists or not
		''' </summary>
		''' <param name="sessionID"> Session ID to check </param>
		''' <returns> true if session exists, false otherwise </returns>
		Function sessionExists(ByVal sessionID As String) As Boolean

		''' <summary>
		''' Get the static info for the given session and worker IDs, or null if no such static info has been reported
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <param name="workerID">  worker ID </param>
		''' <returns> Static info, or null if none has been reported </returns>
		Function getStaticInfo(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Persistable

		''' <summary>
		''' Get all static informations for the given session and type ID
		''' </summary>
		''' <param name="sessionID">    Session ID to get static info for </param>
		''' <param name="typeID">       Type ID to get static info for </param>
		''' <returns>             All static info instances matching both the session and type IDs </returns>
		Function getAllStaticInfos(ByVal sessionID As String, ByVal typeID As String) As IList(Of Persistable)

		''' <summary>
		''' Get the list of type IDs for the given session ID
		''' </summary>
		''' <param name="sessionID"> Session ID to query </param>
		''' <returns> List of type IDs </returns>
		Function listTypeIDsForSession(ByVal sessionID As String) As IList(Of String)

		''' <summary>
		''' For a given session ID, list all of the known worker IDs
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <returns> List of worker IDs, or possibly null if session ID is unknown </returns>
		Function listWorkerIDsForSession(ByVal sessionID As String) As IList(Of String)

		''' <summary>
		''' For a given session ID and type ID, list all of the known worker IDs
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <param name="typeID"> Type ID </param>
		''' <returns> List of worker IDs, or possibly null if session ID is unknown </returns>
		Function listWorkerIDsForSessionAndType(ByVal sessionID As String, ByVal typeID As String) As IList(Of String)

		''' <summary>
		''' Return the number of update records for the given session ID (all workers)
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <returns> number of update records </returns>
		Function getNumUpdateRecordsFor(ByVal sessionID As String) As Integer

		''' <summary>
		''' Return the number of update records for the given session ID and worker ID
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <param name="workerID">  Worker ID </param>
		''' <returns> number of update records </returns>
		Function getNumUpdateRecordsFor(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Integer

		''' <summary>
		''' Get the latest update record (i.e., update record with the largest timestamp value) for the specified
		''' session and worker IDs
		''' </summary>
		''' <param name="sessionID"> session ID </param>
		''' <param name="workerID">  worker ID </param>
		''' <returns> UpdateRecord containing the session/worker IDs, timestamp and content for the most recent update </returns>
		Function getLatestUpdate(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Persistable

		''' <summary>
		''' Get the specified update (or null, if none exists for the given session/worker ids and timestamp)
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <param name="workerID">  Worker ID </param>
		''' <param name="timestamp"> Timestamp </param>
		''' <returns> Update </returns>
		Function getUpdate(ByVal sessionID As String, ByVal typeId As String, ByVal workerID As String, ByVal timestamp As Long) As Persistable

		''' <summary>
		''' Get the latest update for all workers, for the given session ID
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <returns> List of updates for the given Session ID </returns>
		Function getLatestUpdateAllWorkers(ByVal sessionID As String, ByVal typeID As String) As IList(Of Persistable)

		''' <summary>
		''' Get all updates for the given session and worker ID, that occur after (not including) the given timestamp.
		''' Results should be sorted by time.
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <param name="workerID">  Worker Id </param>
		''' <param name="timestamp"> Timestamp </param>
		''' <returns> List of records occurring after the given timestamp </returns>
		Function getAllUpdatesAfter(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamp As Long) As IList(Of Persistable)

		''' <summary>
		''' Get all updates for the given session ID (all worker IDs), that occur after (not including) the given timestamp.
		''' Results should be sorted by time.
		''' </summary>
		''' <param name="sessionID"> Session ID </param>
		''' <param name="timestamp"> Timestamp </param>
		''' <returns> List of records occurring after the given timestamp </returns>
		Function getAllUpdatesAfter(ByVal sessionID As String, ByVal typeID As String, ByVal timestamp As Long) As IList(Of Persistable)

		''' <summary>
		''' List the times of all updates for the specified sessionID, typeID and workerID
		''' </summary>
		''' <param name="sessionID"> Session ID to get update times for </param>
		''' <param name="typeID">    Type ID to get update times for </param>
		''' <param name="workerID">  Worker ID to get update times for </param>
		''' <returns>          Times of all updates </returns>
		Function getAllUpdateTimes(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String) As Long()

		''' <summary>
		''' Get updates for the specified times only
		''' </summary>
		''' <param name="sessionID"> Session ID to get update times for </param>
		''' <param name="typeID">    Type ID to get update times for </param>
		''' <param name="workerID">  Worker ID to get update times for </param>
		''' <param name="timestamps"> Timestamps to get the updates for. Note that if one of the specified times does not exist,
		'''                   it will be ommitted from the returned results list. </param>
		''' <returns>          List of updates at the specified times </returns>
		Function getUpdates(ByVal sessionID As String, ByVal typeID As String, ByVal workerID As String, ByVal timestamps() As Long) As IList(Of Persistable)

		''' <summary>
		''' Get the session metadata, if any has been registered via <seealso cref="putStorageMetaData(StorageMetaData)"/>
		''' </summary>
		''' <param name="sessionID"> Session ID to get metadat </param>
		''' <returns> Session metadata, or null if none is available </returns>
		Function getStorageMetaData(ByVal sessionID As String, ByVal typeID As String) As StorageMetaData

		' ----- Listeners -----

		''' <summary>
		''' Add a new StatsStorageListener. The given listener will called whenever a state change occurs for the stats
		''' storage instance
		''' </summary>
		''' <param name="listener"> Listener to add </param>
		Sub registerStatsStorageListener(ByVal listener As StatsStorageListener)

		''' <summary>
		''' Remove the specified listener, if it is present.
		''' </summary>
		''' <param name="listener"> Listener to remove </param>
		Sub deregisterStatsStorageListener(ByVal listener As StatsStorageListener)

		''' <summary>
		''' Remove all listeners from the StatsStorage instance
		''' </summary>
		Sub removeAllListeners()

		''' <summary>
		''' Get a list (shallow copy) of all listeners currently present
		''' </summary>
		''' <returns> List of listeners </returns>
		ReadOnly Property Listeners As IList(Of StatsStorageListener)

	End Interface

End Namespace