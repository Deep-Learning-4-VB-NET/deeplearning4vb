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

Imports System

Namespace org.datavec.api.io.serializers

    ''' <summary>A serializer/deserializer pair for a family of classes.</summary>
    Public Interface Serialization(Of T)

        ''' <summary>
        ''' Allows clients to test whether this <seealso cref="Serialization(Of T)"/>
        ''' supports the given class.
        ''' </summary>
        Function accept(ByVal c As Type) As Boolean

        ''' <returns> a <seealso cref="Serializer(Of T)"/> for the given class. </returns>
        Function getSerializer(ByVal c As Type) As Serializer(Of T)

        ''' <returns> a <seealso cref="Deserializer(Of T)"/> for the given class. </returns>
        Function getDeserializer(ByVal c As Type) As Deserializer(Of T)

    End Interface

End Namespace
