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



	Public Interface StatsStorageRouter


		''' <summary>
		''' Method to store some additional metadata for each session. Idea: record the classes used to
		''' serialize and deserialize the static info and updates (as a class name).
		''' This is mainly used for debugging and validation.
		''' </summary>
		''' <param name="storageMetaData"> Storage metadata to store </param>
		Sub putStorageMetaData(ByVal storageMetaData As StorageMetaData) 'TODO error handling

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void putStorageMetaData(java.util.Collection<? extends StorageMetaData> storageMetaData);
		Sub putStorageMetaData(Of T1 As StorageMetaData)(ByVal storageMetaData As ICollection(Of T1))

		''' <summary>
		''' Static info: reported once per session, upon initialization
		''' </summary>
		''' <param name="staticInfo">    Static info to store </param>
		Sub putStaticInfo(ByVal staticInfo As Persistable) 'TODO error handling

		''' <summary>
		''' Static info: reported once per session, upon initialization
		''' </summary>
		''' <param name="staticInfo">    Static info to store </param>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void putStaticInfo(java.util.Collection<? extends Persistable> staticInfo);
		Sub putStaticInfo(Of T1 As Persistable)(ByVal staticInfo As ICollection(Of T1))

		''' <summary>
		''' Updates: stored multiple times per session (periodically, for example)
		''' </summary>
		''' <param name="update">    Update info to store </param>
		Sub putUpdate(ByVal update As Persistable) 'TODO error handling

		''' <summary>
		''' Updates: stored multiple times per session (periodically, for example)
		''' </summary>
		''' <param name="updates">    Update info to store </param>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: void putUpdate(java.util.Collection<? extends Persistable> updates);
		Sub putUpdate(Of T1 As Persistable)(ByVal updates As ICollection(Of T1))

	End Interface

End Namespace