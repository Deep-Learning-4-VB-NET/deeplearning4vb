Imports System
Imports Configurable = org.datavec.api.conf.Configurable
Imports Configuration = org.datavec.api.conf.Configuration
Imports DataInputBuffer = org.datavec.api.io.DataInputBuffer
Imports DataOutputBuffer = org.datavec.api.io.DataOutputBuffer
Imports org.datavec.api.io.serializers
Imports SerializationFactory = org.datavec.api.io.serializers.SerializationFactory
Imports org.datavec.api.io.serializers

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

Namespace org.datavec.api.util


	<Obsolete>
	Public Class ReflectionUtils

		Private Shared ReadOnly EMPTY_ARRAY() As Type = {}
		Private Shared serialFactory As SerializationFactory = Nothing

		Private Sub New()
		End Sub

		''' <summary>
		''' Create an object for the given class and initialize it from conf
		''' </summary>
		''' <param name="theClass"> class of which an object is created </param>
		''' <param name="conf"> Configuration </param>
		''' <returns> a new object </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T newInstance(@Class<T> theClass, org.datavec.api.conf.Configuration conf)
		Public Shared Function newInstance(Of T)(ByVal theClass As Type(Of T), ByVal conf As Configuration) As T
			Dim result As T = org.nd4j.common.io.ReflectionUtils.newInstance(theClass)
			setConf(result, conf)
			Return result
		End Function

		''' <summary>
		''' Check and set 'configuration' if necessary.
		''' </summary>
		''' <param name="theObject"> object for which to set configuration </param>
		''' <param name="conf"> Configuration </param>
		Public Shared Sub setConf(ByVal theObject As Object, ByVal conf As Configuration)
			If conf IsNot Nothing Then
				If TypeOf theObject Is Configurable Then
					DirectCast(theObject, Configurable).Conf = conf
				End If
				setJobConf(theObject, conf)
			End If
		End Sub

		''' <summary>
		''' This code is to support backward compatibility and break the compile
		''' time dependency of core on mapred.
		''' This should be made deprecated along with the mapred package HADOOP-1230.
		''' Should be removed when mapred package is removed.
		''' </summary>
		Private Shared Sub setJobConf(ByVal theObject As Object, ByVal conf As Configuration)
			'If JobConf and JobConfigurable are in classpath, AND
			'theObject is of type JobConfigurable AND
			'conf is of type JobConf then
			'invoke configure on theObject
			Try
				Dim jobConfClass As Type = conf.getClassByName("org.apache.hadoop.mapred.JobConf")
				Dim jobConfigurableClass As Type = conf.getClassByName("org.apache.hadoop.mapred.JobConfigurable")
				If jobConfClass.IsAssignableFrom(conf.GetType()) AndAlso jobConfigurableClass.IsAssignableFrom(theObject.GetType()) Then
					Dim configureMethod As System.Reflection.MethodInfo = jobConfigurableClass.GetMethod("configure", jobConfClass)
					configureMethod.invoke(theObject, conf)
				End If
			Catch e As ClassNotFoundException
				'JobConf/JobConfigurable not in classpath. no need to configure
			Catch e As Exception
				Throw New Exception("Error in configuring object", e)
			End Try
		End Sub


		''' <summary>
		''' A pair of input/output buffers that we use to clone writables.
		''' </summary>
		Private Class CopyInCopyOutBuffer
			Friend outBuffer As New DataOutputBuffer()
			Friend inBuffer As New DataInputBuffer()

			''' <summary>
			''' Move the data from the output buffer to the input buffer.
			''' </summary>
			Friend Overridable Sub moveData()
				inBuffer.reset(outBuffer.Data, outBuffer.Length)
			End Sub
		End Class

		''' <summary>
		''' Allocate a buffer for each thread that tries to clone objects.
		''' </summary>
		Private Shared cloneBuffers As ThreadLocal(Of CopyInCopyOutBuffer) = New ThreadLocalAnonymousInnerClass()

		Private Class ThreadLocalAnonymousInnerClass
			Inherits ThreadLocal(Of CopyInCopyOutBuffer)

			Protected Friend Function initialValue() As CopyInCopyOutBuffer
				SyncLock Me
					Return New CopyInCopyOutBuffer()
				End SyncLock
			End Function
		End Class

		Private Shared Function getFactory(ByVal conf As Configuration) As SerializationFactory
			If serialFactory Is Nothing Then
				serialFactory = New SerializationFactory(conf)
			End If
			Return serialFactory
		End Function

		''' <summary>
		''' Make a copy of the writable object using serialization to a buffer </summary>
		''' <param name="dst"> the object to copy from </param>
		''' <param name="src"> the object to copy into, which is destroyed </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T> T copy(org.datavec.api.conf.Configuration conf, T src, T dst) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function copy(Of T)(ByVal conf As Configuration, ByVal src As T, ByVal dst As T) As T
			Dim buffer As CopyInCopyOutBuffer = cloneBuffers.get()
			buffer.outBuffer.reset()
			Dim factory As SerializationFactory = getFactory(conf)
			Dim cls As Type(Of T) = CType(src.GetType(), Type(Of T))
			Dim serializer As Serializer(Of T) = factory.getSerializer(cls)
			serializer.open(buffer.outBuffer)
			serializer.serialize(src)
			buffer.moveData()
			Dim deserializer As Deserializer(Of T) = factory.getDeserializer(cls)
			deserializer.open(buffer.inBuffer)
			dst = deserializer.deserialize(dst)
			Return dst
		End Function
	End Class

End Namespace