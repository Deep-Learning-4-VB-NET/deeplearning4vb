Imports System
Imports Kryo = com.esotericsoftware.kryo.Kryo
Imports Serializer = com.esotericsoftware.kryo.Serializer
Imports Input = com.esotericsoftware.kryo.io.Input
Imports Output = com.esotericsoftware.kryo.io.Output
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble

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

Namespace org.nd4j.kryo.primitives

	Public Class AtomicDoubleSerializer
		Inherits Serializer(Of AtomicDouble)

		Public Overrides Sub write(ByVal kryo As Kryo, ByVal output As Output, ByVal a As AtomicDouble)
			output.writeDouble(a.get())
		End Sub

		Public Overrides Function read(ByVal kryo As Kryo, ByVal input As Input, ByVal a As Type(Of AtomicDouble)) As AtomicDouble
			Return New AtomicDouble(input.readDouble())
		End Function
	End Class

End Namespace