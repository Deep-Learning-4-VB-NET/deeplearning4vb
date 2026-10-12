Imports System
Imports System.Collections.Generic
Imports ExceptionUtils = org.apache.commons.lang3.exception.ExceptionUtils
Imports Configuration = org.datavec.api.conf.Configuration
Imports Configured = org.datavec.api.conf.Configured
Imports ReflectionUtils = org.datavec.api.util.ReflectionUtils
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

Namespace org.datavec.api.io.serializers



	Public Class SerializationFactory
		Inherits Configured

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared ReadOnly LOG As Logger = LoggerFactory.getLogger(GetType(SerializationFactory).FullName)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private java.util.List<Serialization<?>> serializations = new java.util.ArrayList<>();
		Private serializations As IList(Of Serialization(Of Object)) = New List(Of Serialization(Of Object))()

		''' <summary>
		''' <para>
		''' Serializations are found by reading the <code>io.serializations</code>
		''' property from <code>conf</code>, which is a comma-delimited list of
		''' classnames.
		''' </para>
		''' </summary>
		Public Sub New(ByVal conf As Configuration)
			MyBase.New(conf)
			For Each serializerName As String In conf.getStrings("io.serializations", New String() {"org.apache.hadoop.io.serializer.WritableSerialization"})
				add(conf, serializerName)
			Next serializerName
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") private void add(org.datavec.api.conf.Configuration conf, String serializationName)
		Private Sub add(ByVal conf As Configuration, ByVal serializationName As String)
			Try

				Dim serializationClass As Type = CType(conf.getClassByName(serializationName), Type)
				serializations.Add(ReflectionUtils.newInstance(serializationClass, Me.Conf))
			Catch e As ClassNotFoundException
				LOG.warn("Serialization class not found: " & ExceptionUtils.getStackTrace(e))
			End Try
		End Sub

		Public Overridable Function getSerializer(Of T)(ByVal c As Type(Of T)) As Serializer(Of T)
			Return getSerialization(c).getSerializer(c)
		End Function

		Public Overridable Function getDeserializer(Of T)(ByVal c As Type(Of T)) As Deserializer(Of T)
			Return getSerialization(c).getDeserializer(c)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> Serialization<T> getSerialization(@Class<T> c)
		Public Overridable Function getSerialization(Of T)(ByVal c As Type(Of T)) As Serialization(Of T)
			For Each serialization As Serialization In serializations
				If serialization.accept(c) Then
					Return CType(serialization, Serialization(Of T))
				End If
			Next serialization
			Return Nothing
		End Function
	End Class

End Namespace