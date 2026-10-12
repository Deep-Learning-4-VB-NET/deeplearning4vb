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


	Public Interface StorageMetaData
		Inherits Persistable

		''' <summary>
		''' Timestamp for the metadata
		''' </summary>
		ReadOnly Property TimeStamp As Long

		''' <summary>
		''' Session ID for the metadata
		''' </summary>
		ReadOnly Property SessionID As String

		''' <summary>
		''' Type ID for the metadata
		''' </summary>
		ReadOnly Property TypeID As String

		''' <summary>
		''' Worker ID for the metadata
		''' </summary>
		ReadOnly Property WorkerID As String

		''' <summary>
		''' Full class name for the initialization information that will be posted. Is expected to implement <seealso cref="Persistable"/>.
		''' </summary>
		ReadOnly Property InitTypeClass As String

		''' <summary>
		''' Full class name for the update information that will be posted. Is expected to implement <seealso cref="Persistable"/>.
		''' </summary>
		ReadOnly Property UpdateTypeClass As String

		''' <summary>
		''' Get extra metadata, if any
		''' </summary>
		ReadOnly Property ExtraMetaData As Serializable

	End Interface

End Namespace