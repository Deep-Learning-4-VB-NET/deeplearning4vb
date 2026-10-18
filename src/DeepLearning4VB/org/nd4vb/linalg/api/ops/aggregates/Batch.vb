Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException

Namespace org.nd4vb.linalg.api.ops.aggregates

    ''' <summary>
    ''' Wrapper for "batch of aggregates": up to <see cref="Batch.getBatchLimit"/> aggregates of one kind executed
    ''' together. The static members of the Java class (<c>getBatches</c>, <c>getBatchLimit</c>) are on the
    ''' non-generic companion class <see cref="Batch"/>, so they can be called without type arguments.
    ''' </summary>
    Public Class Batch(Of T As Aggregate)

        ''' <summary>
        ''' This batch will be allocated on device, only if we're using GPU backend
        ''' </summary>
        Private paramsSurface As DataBuffer

        ' all aggregates within this batch
        Private aggregates As IList(Of T)

        Private sample As T
        Private numAggregates As Integer

        ''' <summary>
        ''' This constructor takes List of Aggregates, and builds Batch instance, usable with Nd4j executioner.
        ''' </summary>
        ''' <param name="aggregates"> </param>
        Public Sub New(ByVal aggregates As IList(Of T))
            'if (aggregates.size() > batchLimit)
            '    throw new RuntimeException("Number of aggregates is higher then " + batchLimit + " elements, multiple batches should be issued.");

            Me.aggregates = aggregates
            Me.numAggregates = aggregates.Count

            ' we fetch single sample for possible future use. not sure if will be used though
            Me.sample = aggregates(0)
        End Sub

        Public Overridable Function getParamsSurface() As DataBuffer
            Return paramsSurface
        End Function

        Public Overridable Sub setParamsSurface(ByVal paramsSurface As DataBuffer)
            Me.paramsSurface = paramsSurface
        End Sub

        Public Overridable Function getAggregates() As IList(Of T)
            Return aggregates
        End Function

        Public Overridable Function getSample() As T
            Return sample
        End Function

        Public Overridable Function getNumAggregates() As Integer
            Return numAggregates
        End Function

        ''' <summary>
        ''' This method returns opNum for batched aggregate
        ''' </summary>
        Public Overridable Function opNum() As Integer
            Return sample.opNum()
        End Function

        ''' <summary>
        ''' This method tries to append aggregate to the current batch, if it has free room
        ''' </summary>
        ''' <param name="aggregate"> </param>
        ''' <returns> True if the aggregate was added </returns>
        Public Overridable Function append(ByVal aggregate As T) As Boolean
            If Not Full Then
                aggregates.Add(aggregate)
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' This method checks, if number of batched aggregates equals to maximum possible value
        ''' </summary>
        Public Overridable ReadOnly Property Full As Boolean
            Get
                Return Batch.getBatchLimit() = numAggregates
            End Get
        End Property

    End Class

    ''' <summary>The static members of <see cref="Batch(Of T)"/>.</summary>
    Public NotInheritable Class Batch

        Private Const batchLimit As Integer = 512

        Private Sub New()
        End Sub

        ''' <summary>The maximum number of aggregates in one batch (512).</summary>
        Public Shared Function getBatchLimit() As Integer
            Return batchLimit
        End Function

        ''' <summary>
        ''' Helper method to create batch from list of aggregates, for cases when list of aggregates is higher then batchLimit
        ''' </summary>
        ''' <param name="list"> </param>
        Public Shared Function getBatches(Of U As Aggregate)(ByVal list As IList(Of U)) As IList(Of Batch(Of U))
            Return getBatches(list, batchLimit)
        End Function

        ''' <summary>
        ''' Helper method to create batch from list of aggregates, for cases when list of aggregates is higher then batchLimit
        ''' </summary>
        ''' <param name="list"> </param>
        ''' <param name="partitionSize"> the maximum size of one batch </param>
        Public Shared Function getBatches(Of U As Aggregate)(ByVal list As IList(Of U), ByVal partitionSize As Integer) As IList(Of Batch(Of U))
            Dim c As DataType? = Nothing
            For Each item As U In list
                For Each a In item.Arguments
                    ' we'll be comparing to the first array
                    If Not c.HasValue AndAlso a IsNot Nothing Then
                        c = a.dataType()
                    End If

                    If a IsNot Nothing AndAlso c.HasValue Then
                        Preconditions.checkArgument(c.Value = a.dataType(), "All arguments must have same data type")
                    End If
                Next a
            Next item

            If Not c.HasValue Then
                Throw New ND4VBIllegalStateException("Can't infer data type from arguments")
            End If

            ' Guava Lists.partition: consecutive sublists of partitionSize elements (the last may be smaller)
            Dim split As IList(Of Batch(Of U)) = New List(Of Batch(Of U))()
            For start As Integer = 0 To list.Count - 1 Step partitionSize
                Dim partition As New List(Of U)()
                For i As Integer = start To Math.Min(start + partitionSize, list.Count) - 1
                    partition.Add(list(i))
                Next i
                split.Add(New Batch(Of U)(partition))
            Next start

            Return split
        End Function

    End Class

End Namespace
