Imports System
Imports System.Reflection
Imports DifferentialFunction = DeepLearning4VB.org.nd4vb.autodiff.functions.DifferentialFunction

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

Namespace org.nd4vb.imports.descriptors.properties

    ''' <summary>
    ''' Adapts an attribute value of an imported model (TensorFlow or ONNX) to the type of a field of an op
    ''' (Java <c>AttributeAdapter</c>). Java's <c>java.lang.reflect.Field</c> is <see cref="FieldInfo"/>.
    ''' </summary>
    Public Interface AttributeAdapter

        ''' <summary>
        ''' Map the attribute using the specified field
        ''' on the specified function on
        ''' adapting the given input type to
        ''' the type of the field for the specified function. </summary>
        ''' <param name="inputAttributeValue"> the evaluate to adapt </param>
        ''' <param name="fieldFor"> the field for </param>
        ''' <param name="on"> the function to map on </param>
        Sub mapAttributeFor(ByVal inputAttributeValue As Object, ByVal fieldFor As FieldInfo, ByVal [on] As DifferentialFunction)

    End Interface

End Namespace
