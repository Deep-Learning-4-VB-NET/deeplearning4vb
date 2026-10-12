Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Configuration = org.apache.hadoop.conf.Configuration

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

Namespace org.datavec.spark.util


	<Serializable>
	Public Class SerializableHadoopConfig

		Private content As IDictionary(Of String, String)
'JAVA TO VB CONVERTER NOTE: The field configuration was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private configuration_Conflict As Configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SerializableHadoopConfig(@NonNull Configuration configuration)
		Public Sub New(ByVal configuration As Configuration)
			Me.configuration_Conflict = configuration
			content = New LinkedHashMap(Of String, String)()
			Dim iter As Configuration.Enumerator = configuration.GetEnumerator()
			Do While iter.MoveNext()
				Dim [next] As KeyValuePair(Of String, String) = iter.Current
				content([next].Key) = [next].Value
			Loop
		End Sub

		Public Overridable ReadOnly Property Configuration As Configuration
			Get
				SyncLock Me
					If configuration_Conflict Is Nothing Then
						configuration_Conflict = New Configuration()
						For Each e As KeyValuePair(Of String, String) In content.SetOfKeyValuePairs()
							configuration_Conflict.set(e.Key, e.Value)
						Next e
					End If
					Return configuration_Conflict
				End SyncLock
			End Get
		End Property

	End Class

End Namespace