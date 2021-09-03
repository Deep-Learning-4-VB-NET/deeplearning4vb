Imports System
Imports Kryo = com.esotericsoftware.kryo.Kryo
Imports Serializer = com.esotericsoftware.kryo.Serializer
Imports Input = com.esotericsoftware.kryo.io.Input
Imports Output = com.esotericsoftware.kryo.io.Output
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.kryo


	Public Class Nd4jSerializer
		Inherits Serializer(Of INDArray)

		''' <summary>
		''' Writes the bytes for the object to the output.
		''' <para>
		''' This method should not be called directly, instead this serializer can be passed to <seealso cref="Kryo"/> write methods that accept a
		''' serialier.
		''' 
		''' </para>
		''' </summary>
		''' <param name="kryo"> </param>
		''' <param name="output"> </param>
		''' <param name="object"> May be null if <seealso cref="getAcceptsNull()"/> is true. </param>
		Public Overrides Sub write(ByVal kryo As Kryo, ByVal output As Output, ByVal [object] As INDArray)
			Dim dos As New DataOutputStream(output)
			Try
				Nd4j.write([object], dos)
			Catch e As IOException
				Throw New Exception(e)
			End Try
			'Note: output should NOT be closed manually here - may be needed elsewhere (and closing here will cause serialization to fail)
		End Sub

		''' <summary>
		''' Reads bytes and returns a new object of the specified concrete opType.
		''' <para>
		''' Before Kryo can be used to read child objects, <seealso cref="Kryo.reference(Object)"/> must be called with the parent object to
		''' ensure it can be referenced by the child objects. Any serializer that uses <seealso cref="Kryo"/> to read a child object may need to
		''' be reentrant.
		''' </para>
		''' <para>
		''' This method should not be called directly, instead this serializer can be passed to <seealso cref="Kryo"/> read methods that accept a
		''' serialier.
		''' 
		''' </para>
		''' </summary>
		''' <param name="kryo"> </param>
		''' <param name="input"> </param>
		''' <param name="type"> </param>
		''' <returns> May be null if <seealso cref="getAcceptsNull()"/> is true. </returns>
		Public Overrides Function read(ByVal kryo As Kryo, ByVal input As Input, ByVal type As Type(Of INDArray)) As INDArray
			Dim dis As New DataInputStream(input)
			Return Nd4j.read(dis)
			'Note: input should NOT be closed manually here - may be needed elsewhere (and closing here will cause serialization to fail)
		End Function



	End Class

End Namespace