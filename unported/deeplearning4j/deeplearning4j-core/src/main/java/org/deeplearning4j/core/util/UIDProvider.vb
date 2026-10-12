Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Slf4j = lombok.extern.slf4j.Slf4j

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

Namespace org.deeplearning4j.core.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class UIDProvider
	Public Class UIDProvider

		Private Shared ReadOnly JVM_UID As String
		Private Shared ReadOnly HARDWARE_UID As String

		Shared Sub New()

			Dim jvmUIDSource As New UID()
			Dim asString As String = jvmUIDSource.ToString()
			'Format here: hexStringFromRandomNumber:hexStringFromSystemClock:hexStringOfUIDInstance
			'The first two components here will be identical for all UID instances in a JVM, where as the 'hexStringOfUIDInstance'
			' will vary (increment) between UID object instances. So we'll only be using the first two components here
			Dim lastIdx As Integer = asString.LastIndexOf(":", StringComparison.Ordinal)
			JVM_UID = asString.Substring(0, lastIdx).replaceAll(":", "")


			'Assumptions here:
			'1. getNetworkInterfaces() returns at least one non-null element
			'   This is guaranteed by getNetworkInterfaces() Javadoc: "The {@code Enumeration} contains at least one element..."
			'2. That the iteration order for network interfaces is consistent between JVM instances on the same hardware
			'   This appears to hold, but no formal guarantees seem to be available here
			'3. That MAC addresses are 'unique enough' for our purposes
			Dim address() As SByte = Nothing
			Dim noInterfaces As Boolean = False
			Dim niEnumeration As IEnumerator(Of NetworkInterface) = Nothing
			Try
				niEnumeration = NetworkInterface.getNetworkInterfaces()
			Catch e As Exception
				noInterfaces = True
			End Try

			If niEnumeration IsNot Nothing Then
				Do While niEnumeration.MoveNext()
					Dim ni As NetworkInterface = niEnumeration.Current
					Dim addr() As SByte
					Try
						addr = ni.getHardwareAddress()
					Catch e As Exception
						Continue Do
					End Try
					If addr Is Nothing OrElse addr.Length <> 6 Then
						Continue Do 'May be null (if it can't be obtained) or not standard 6 byte MAC-48 representation
					End If

					address = addr
					Exit Do
				Loop
			End If

			If address Is Nothing Then
				log.warn("Could not generate hardware UID{}. Using fallback: JVM UID as hardware UID.", (If(noInterfaces, " (no interfaces)", "")))
				HARDWARE_UID = JVM_UID
			Else
				Dim sb As New StringBuilder()
				For Each b As SByte In address
					sb.Append(String.Format("{0:x2}", b))
				Next b
				HARDWARE_UID = sb.ToString()
			End If
		End Sub

		Private Sub New()
		End Sub


		Public Shared ReadOnly Property JVMUID As String
			Get
				Return JVM_UID
			End Get
		End Property

		Public Shared ReadOnly Property HardwareUID As String
			Get
				Return HARDWARE_UID
			End Get
		End Property



	End Class

End Namespace