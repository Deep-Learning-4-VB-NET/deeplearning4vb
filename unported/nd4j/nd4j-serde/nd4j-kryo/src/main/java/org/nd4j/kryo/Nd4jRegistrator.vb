Imports Kryo = com.esotericsoftware.kryo.Kryo
Imports SynchronizedCollectionsSerializer = de.javakaffee.kryoserializers.SynchronizedCollectionsSerializer
Imports UnmodifiableCollectionsSerializer = de.javakaffee.kryoserializers.UnmodifiableCollectionsSerializer
Imports KryoRegistrator = org.apache.spark.serializer.KryoRegistrator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble
Imports AtomicDoubleSerializer = org.nd4j.kryo.primitives.AtomicDoubleSerializer

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

	Public Class Nd4jRegistrator
		Implements KryoRegistrator

		Public Overrides Sub registerClasses(ByVal kryo As Kryo)
			kryo.register(Nd4j.Backend.NDArrayClass, New Nd4jSerializer())
			kryo.register(GetType(AtomicDouble), New AtomicDoubleSerializer())

			'Also register Java types (synchronized/unmodifiable collections), which will fail by default
			UnmodifiableCollectionsSerializer.registerSerializers(kryo)
			SynchronizedCollectionsSerializer.registerSerializers(kryo)
		End Sub
	End Class

End Namespace