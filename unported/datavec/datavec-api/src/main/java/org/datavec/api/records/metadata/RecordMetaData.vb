Imports System

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

Namespace org.datavec.api.records.metadata


	Public Interface RecordMetaData

		''' <summary>
		''' Get a human-readable location for the data
		''' </summary>
		ReadOnly Property Location As String

		''' <summary>
		''' Return the URI for the source of the record
		''' </summary>
		''' <returns> The URI for the record (file, etc) - or null otherwise </returns>
		ReadOnly Property URI As URI

		''' <summary>
		''' Get the class that was used to generate the record
		''' </summary>
		ReadOnly Property ReaderClass As Type

	End Interface

End Namespace