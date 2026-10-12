Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports ShapeDescriptor = org.nd4j.linalg.api.shape.ShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.jita.constant


	''' <summary>
	''' This class implements storage singleton, to guarantee constant buffers persistence
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class ConstantProtector
		Private Shared ourInstance As New ConstantProtector()

		Public Shared ReadOnly Property Instance As ConstantProtector
			Get
				Return ourInstance
			End Get
		End Property

		Private protectorLegacy As IList(Of DataBuffer) = New CopyOnWriteArrayList(Of DataBuffer)()
		Private protector As IList(Of Pair(Of DataBuffer, Long())) = New CopyOnWriteArrayList(Of Pair(Of DataBuffer, Long()))()
		Private deviceCache As IList(Of IDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long()))) = New List(Of IDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long())))()


		Private Sub New()
			purgeProtector()
		End Sub

		Public Overridable Sub purgeProtector()
			protector = New CopyOnWriteArrayList(Of Pair(Of DataBuffer, Long()))()
			deviceCache = New List(Of IDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long())))()

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

			For i As Integer = 0 To numDevices - 1
				deviceCache.Insert(i, New ConcurrentDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long()))())
			Next i
		End Sub

		Public Overridable Sub persistDataBuffer(ByVal buffer As DataBuffer)
			protectorLegacy.Add(buffer)
		End Sub

		Public Overridable Sub persistDataBuffer(ByVal buffer As Pair(Of DataBuffer, Long()))
			protector.Add(buffer)
		End Sub

		Public Overridable Sub persistDataBuffer(ByVal deviceId As Integer, ByVal descriptor As ShapeDescriptor, ByVal buffer As Pair(Of DataBuffer, Long()))
			deviceCache(deviceId)(LongShapeDescriptor.fromShapeDescriptor(descriptor)) = buffer
		End Sub

		Public Overridable Sub persistDataBuffer(ByVal deviceId As Integer, ByVal descriptor As LongShapeDescriptor, ByVal buffer As Pair(Of DataBuffer, Long()))
			deviceCache(deviceId)(descriptor) = buffer
		End Sub

		Public Overridable Function getDataBuffer(ByVal deviceId As Integer, ByVal descriptor As ShapeDescriptor) As Pair(Of DataBuffer, Long())
			Return deviceCache(deviceId)(LongShapeDescriptor.fromShapeDescriptor(descriptor))
		End Function

		Public Overridable Function getDataBuffer(ByVal deviceId As Integer, ByVal descriptor As LongShapeDescriptor) As Pair(Of DataBuffer, Long())
			Return deviceCache(deviceId)(descriptor)
		End Function

		Public Overridable Function containsDataBuffer(ByVal deviceId As Integer, ByVal descriptor As ShapeDescriptor) As Boolean
			Return deviceCache(deviceId).ContainsKey(LongShapeDescriptor.fromShapeDescriptor(descriptor))
		End Function

		Public Overridable Function containsDataBuffer(ByVal deviceId As Integer, ByVal descriptor As LongShapeDescriptor) As Boolean
			Return deviceCache(deviceId).ContainsKey(descriptor)
		End Function


	End Class

End Namespace