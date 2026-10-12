Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull

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

Namespace org.nd4j.parameterserver.distributed.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data public class NetworkInformation implements java.io.Serializable
	<Serializable>
	Public Class NetworkInformation
		Protected Friend totalMemory As Long = 0
		Protected Friend availableMemory As Long = 0
		Protected Friend ipAddresses As IList(Of String) = New List(Of String)()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addIpAddress(@NonNull String ip)
		Public Overridable Sub addIpAddress(ByVal ip As String)
			ipAddresses.Add(ip)
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As NetworkInformation = DirectCast(o, NetworkInformation)

'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return ipAddresses != null ? ipAddresses.equals(that.ipAddresses) : that.ipAddresses == null;
			Return If(ipAddresses IsNot Nothing, ipAddresses.SequenceEqual(that.ipAddresses), that.ipAddresses Is Nothing)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(ipAddresses IsNot Nothing, ipAddresses.GetHashCode(), 0)
		End Function
	End Class

End Namespace