Imports System
Imports System.Text

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
    ''' How a property (field) of an op maps to the attributes and inputs of an imported TensorFlow or ONNX node
    ''' (Java <c>PropertyMapping</c>). Op classes return these from <c>mappingsForFunction()</c>.
    ''' <para>
    ''' Lombok's <c>@Data</c> and <c>@Builder</c> members are written out: each field is a property with the Java
    ''' accessors next to it, <see cref="builder"/> returns a <see cref="PropertyMappingBuilder"/>, and
    ''' <see cref="Equals"/>, <see cref="GetHashCode"/> and <see cref="ToString"/> compare and render every field
    ''' (arrays by content), as Lombok does.
    ''' </para>
    ''' </summary>
    <Serializable>
    Public Class PropertyMapping

        Private propertyNamesField() As String
        Private tfInputPositionField As Integer?
        Private onnxAttrNameField As String
        Private tfAttrNameField As String
        Private shapePositionField As Integer?

        ''' <summary>Creates a mapping with every field set (Lombok's all-arguments constructor for the builder).</summary>
        Public Sub New(ByVal propertyNames() As String, ByVal tfInputPosition As Integer?, ByVal onnxAttrName As String, ByVal tfAttrName As String, ByVal shapePosition As Integer?)
            Me.propertyNamesField = propertyNames
            Me.tfInputPositionField = tfInputPosition
            Me.onnxAttrNameField = onnxAttrName
            Me.tfAttrNameField = tfAttrName
            Me.shapePositionField = shapePosition
        End Sub

        ''' <summary>Returns a builder for a mapping.</summary>
        Public Shared Function builder() As PropertyMappingBuilder
            Return New PropertyMappingBuilder()
        End Function

        ''' <summary>The names of the op properties (fields) this mapping sets.</summary>
        Public Property PropertyNames As String()
            Get
                Return propertyNamesField
            End Get
            Set(ByVal value As String())
                propertyNamesField = value
            End Set
        End Property

        ''' <summary>The position of the TensorFlow node input that holds the value, or Nothing.</summary>
        Public Property TfInputPosition As Integer?
            Get
                Return tfInputPositionField
            End Get
            Set(ByVal value As Integer?)
                tfInputPositionField = value
            End Set
        End Property

        ''' <summary>The name of the ONNX attribute that holds the value, or Nothing.</summary>
        Public Property OnnxAttrName As String
            Get
                Return onnxAttrNameField
            End Get
            Set(ByVal value As String)
                onnxAttrNameField = value
            End Set
        End Property

        ''' <summary>The name of the TensorFlow attribute that holds the value, or Nothing.</summary>
        Public Property TfAttrName As String
            Get
                Return tfAttrNameField
            End Get
            Set(ByVal value As String)
                tfAttrNameField = value
            End Set
        End Property

        ''' <summary>The position in a shape that holds the value, or Nothing.</summary>
        Public Property ShapePosition As Integer?
            Get
                Return shapePositionField
            End Get
            Set(ByVal value As Integer?)
                shapePositionField = value
            End Set
        End Property

        Public Overridable Function getPropertyNames() As String()
            Return propertyNamesField
        End Function

        Public Overridable Sub setPropertyNames(ByVal propertyNames() As String)
            propertyNamesField = propertyNames
        End Sub

        Public Overridable Function getTfInputPosition() As Integer?
            Return tfInputPositionField
        End Function

        Public Overridable Sub setTfInputPosition(ByVal tfInputPosition As Integer?)
            tfInputPositionField = tfInputPosition
        End Sub

        Public Overridable Function getOnnxAttrName() As String
            Return onnxAttrNameField
        End Function

        Public Overridable Sub setOnnxAttrName(ByVal onnxAttrName As String)
            onnxAttrNameField = onnxAttrName
        End Sub

        Public Overridable Function getTfAttrName() As String
            Return tfAttrNameField
        End Function

        Public Overridable Sub setTfAttrName(ByVal tfAttrName As String)
            tfAttrNameField = tfAttrName
        End Sub

        Public Overridable Function getShapePosition() As Integer?
            Return shapePositionField
        End Function

        Public Overridable Sub setShapePosition(ByVal shapePosition As Integer?)
            shapePositionField = shapePosition
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As PropertyMapping = TryCast(o, PropertyMapping)
            If other Is Nothing Then
                Return False
            End If
            Return Arrays.equals(propertyNamesField, other.propertyNamesField) AndAlso
                Nullable.Equals(tfInputPositionField, other.tfInputPositionField) AndAlso
                String.Equals(onnxAttrNameField, other.onnxAttrNameField) AndAlso
                String.Equals(tfAttrNameField, other.tfAttrNameField) AndAlso
                Nullable.Equals(shapePositionField, other.shapePositionField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Const PRIME As Integer = 59
            Dim result As Integer = 1
            result = result * PRIME + Arrays.hashCode(propertyNamesField)
            result = result * PRIME + If(tfInputPositionField.HasValue, tfInputPositionField.Value, 43)
            result = result * PRIME + If(onnxAttrNameField Is Nothing, 43, onnxAttrNameField.GetHashCode())
            result = result * PRIME + If(tfAttrNameField Is Nothing, 43, tfAttrNameField.GetHashCode())
            result = result * PRIME + If(shapePositionField.HasValue, shapePositionField.Value, 43)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder("PropertyMapping(propertyNames=")
            sb.Append(Arrays.toString(propertyNamesField))
            sb.Append(", tfInputPosition=").Append(If(tfInputPositionField.HasValue, tfInputPositionField.Value.ToString(), "null"))
            sb.Append(", onnxAttrName=").Append(If(onnxAttrNameField, "null"))
            sb.Append(", tfAttrName=").Append(If(tfAttrNameField, "null"))
            sb.Append(", shapePosition=").Append(If(shapePositionField.HasValue, shapePositionField.Value.ToString(), "null"))
            sb.Append(")"c)
            Return sb.ToString()
        End Function

        ''' <summary>Lombok's builder for <see cref="PropertyMapping"/>.</summary>
        Public Class PropertyMappingBuilder

            Private propertyNamesValue() As String
            Private tfInputPositionValue As Integer?
            Private onnxAttrNameValue As String
            Private tfAttrNameValue As String
            Private shapePositionValue As Integer?

            Friend Sub New()
            End Sub

            Public Function propertyNames(ParamArray ByVal value() As String) As PropertyMappingBuilder
                propertyNamesValue = value
                Return Me
            End Function

            Public Function tfInputPosition(ByVal value As Integer?) As PropertyMappingBuilder
                tfInputPositionValue = value
                Return Me
            End Function

            Public Function onnxAttrName(ByVal value As String) As PropertyMappingBuilder
                onnxAttrNameValue = value
                Return Me
            End Function

            Public Function tfAttrName(ByVal value As String) As PropertyMappingBuilder
                tfAttrNameValue = value
                Return Me
            End Function

            Public Function shapePosition(ByVal value As Integer?) As PropertyMappingBuilder
                shapePositionValue = value
                Return Me
            End Function

            Public Function build() As PropertyMapping
                Return New PropertyMapping(propertyNamesValue, tfInputPositionValue, onnxAttrNameValue, tfAttrNameValue, shapePositionValue)
            End Function

        End Class

    End Class

End Namespace
